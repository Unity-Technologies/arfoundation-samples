TEXTURE2D(_EnvironmentConfidenceTexture);
SAMPLER(sampler_EnvironmentConfidenceTexture);

TEXTURE2D_ARRAY_FLOAT(_EnvironmentDepthTexture);
SAMPLER(sampler_EnvironmentDepthTexture);
float2 _EnvironmentDepthTexture_TexelSize;

float4x4 _EnvironmentDepthProjectionMatrices[2];
float3 _EnvironmentDepthNearFarPlanes;

float4 DebugDepthDistance(const float depth)
{
    const float4 colors[9] =
    {
        float4(1, 1, 1, 1),        // White, 0 meter
        float4(1, 0, 0, 1),        // Red, 1 meter
        float4(1, 0.125, 0, 1),    // Orange, 2 meters
        float4(1, 1, 0, 1),        // Yellow, 3 meters
        float4(0, 1, 0, 1),        // Green, 4 meters
        float4(0.5, 0.5, 0, 1),    // Cyan, 5 meters
        float4(0, 0, 1, 1),        // Blue, 6 meters
        float4(0.375, 0.375, 1, 1), // Magenta, 7 meters
        float4(0.7, 0.25, 1, 1)   // Pink, 8 meters
    };

    float step = 1;
    float tempD = step;

    while (tempD < depth && tempD < 10)
    {
        tempD += step;
    }

    int colIndex = (int) (tempD / step) - 1;
    colIndex = clamp(colIndex, 0, 7);
    return lerp(colors[colIndex], colors[colIndex + 1], (1 - (tempD - depth)) / step);
    //return colors[colIndex];
}

int isInfinity(const float val, const float infinityThreshold)
{
    return int(val >= infinityThreshold);
}

void SetOcclusionVertOutputs(float4 positionOS, inout float4 positionCS, inout float runtimeDepth,
    inout float4 depthSpaceScreenPosition)
{
    const float4 objectPositionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz, 1.0));
    positionCS = mul(UNITY_MATRIX_VP, objectPositionWS);
    const float4 depthHCS = mul(_EnvironmentDepthProjectionMatrices[unity_StereoEyeIndex], objectPositionWS);
    runtimeDepth = 1 - positionCS.z / positionCS.w;
    depthSpaceScreenPosition = ComputeScreenPos(depthHCS);
}

float NormalizeWithinBounds(const float v, const float min, const float max)
{
    return clamp((v - min) / (max - min), 0, 1);
}

float LinearizeDepth(const float depth, const float near, const float far, const float infinityThreshold)
{
    const float finiteFarRuntimeLinearDepth = near * far / (far - depth * (far - near));
    const float infiniteFarRuntimeLinearDepth = near / (1 - depth);
    const int isInfiniteFar = isInfinity(far, infinityThreshold);

    return infiniteFarRuntimeLinearDepth * isInfiniteFar + finiteFarRuntimeLinearDepth * (1 - isInfiniteFar);
}

float SampleEnvironmentDepth(const float2 uv)
{
    return SAMPLE_TEXTURE2D_ARRAY(_EnvironmentDepthTexture, sampler_EnvironmentDepthTexture, uv, unity_StereoEyeIndex).r;
}

float SampleEnvironmentDepthLinear(const float2 uv)
{
    const float environmentDepth = SampleEnvironmentDepth(uv);

    #ifdef XR_LINEAR_DEPTH
        // depth is already linear
        return environmentDepth;
    #else
        // convert NDC to linear depth
        const float near = _EnvironmentDepthNearFarPlanes.x;
        const float far = _EnvironmentDepthNearFarPlanes.y;
        const float infinityThreshold = _EnvironmentDepthNearFarPlanes.z;

        return LinearizeDepth(environmentDepth, near, far, infinityThreshold);
    #endif
}

float4 SampleEnvironmentDepthGeneral(const float2 uv)
{
    #ifdef XR_SOFT_OCCLUSION
        // replace with sampling for soft occlusion
        return float4(0, 0, 0, 0);
    #else
        return float4(SampleEnvironmentDepthLinear(uv), 0, 0, 0);
    #endif
}

float GetHardPixelVisibility(const float linearEnvironmentDepth, const float linearSceneDepth)
{
    return float (linearEnvironmentDepth > linearSceneDepth);
}

float GetToleranceBasedPixelVisibility(const float linearEnvironmentDepth, const float linearSceneDepth)
{
    const float depthNearMin = 0.0;
    const float depthNearMax = 0.05;
    const float depthFarMin = 3.5;
    const float depthFarMax = 5.5;
    const float depthCloseToleranceThreshold = 3.5;
    const float depthFarToleranceThreshold = 5.5;
    const float toleranceClose = 0.02;
    const float toleranceFurthest = 0.5;
    const float toleranceGamma = 1;

    const float delta = linearSceneDepth - linearEnvironmentDepth;

    // |____|_______|____________|_____|
    // 0  nMin   nMax         fmin   fmax
    //d                            ^

    const float trustDepthNear = NormalizeWithinBounds(linearEnvironmentDepth, depthNearMin, depthNearMax);
    const float trustDepthFar = 1 - NormalizeWithinBounds(linearEnvironmentDepth, depthFarMin, depthFarMax);
    const float sceneAssetVisibility = 1 - NormalizeWithinBounds(linearSceneDepth, depthFarMin, depthFarMax);
    const float tolerance_t = NormalizeWithinBounds(linearEnvironmentDepth, depthCloseToleranceThreshold, depthFarToleranceThreshold);
    const float tolerance = toleranceClose + pow(tolerance_t, toleranceGamma) * (toleranceFurthest - toleranceClose);

    //gradually change visibility 0 to 1 on depth delta values <= tolerance.
    const float closeProximityVisibility = clamp(1 - (delta + tolerance) / (2 * tolerance) * trustDepthFar, 0, 1);
    return sceneAssetVisibility * max(max(closeProximityVisibility, 0), 1 - trustDepthNear);
}

float ComputePixelVisibility(const float4 linearEnvironmentDepth, const float linearSceneDepth)
{
    #ifdef XR_SOFT_OCCLUSION
        // a place for creativity
    #elif XR_HARD_OCCLUSION
        return GetHardPixelVisibility(linearEnvironmentDepth.x, linearSceneDepth);
    #else
        return GetToleranceBasedPixelVisibility(linearEnvironmentDepth.x, linearSceneDepth);
    #endif

    return 1;
}

void SetOcclusion(float4 depthSpaceScreenPosition, float sceneDepth, inout float4 color)
{
    const float2 uv = float2(depthSpaceScreenPosition.x / depthSpaceScreenPosition.w,
        1 - depthSpaceScreenPosition.y / depthSpaceScreenPosition.w) - _EnvironmentDepthTexture_TexelSize * 0.5;

    if (all(uv < 0.0) || all(uv > 1.0))
    {
        return;
    }

    const float infinityThreshold = _EnvironmentDepthNearFarPlanes.z;
    const float linearSceneDepth = LinearizeDepth(sceneDepth, _ProjectionParams.y, _ProjectionParams.z, infinityThreshold);

    const float4 linearEnvironmentDepth = SampleEnvironmentDepthGeneral(uv);

    color.a *= ComputePixelVisibility(linearEnvironmentDepth, linearSceneDepth);
    color.rgb *= color.a;
}
