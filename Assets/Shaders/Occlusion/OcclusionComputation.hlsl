TEXTURE2D(_PassthroughConfidenceTexture);
SAMPLER(sampler_PassthroughConfidenceTexture);
TEXTURE2D_ARRAY(_PassthroughDepthTexture);
SAMPLER(sampler_PassthroughDepthTexture);
float4x4 _DepthVPMatrices[2];

float4 DebugDepthDistance(const float depth)
{
    const float4 colors[8] = 
    { 
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
    colIndex = clamp(colIndex, 0, 6);
    return lerp(colors[colIndex], colors[colIndex + 1], (1 - (tempD - depth)) / step);
    //return colors[colIndex];
}

void SetOcclusionVertOutputs(float4 positionOS, inout float4 positionCS, inout float runtimeDepth,
    inout float4 depthSpaceScreenPosition)
{
    const float4 objectPositionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz, 1.0));
    positionCS = mul(UNITY_MATRIX_VP, objectPositionWS);
    const float4 depthHCS = mul(_DepthVPMatrices[unity_StereoEyeIndex], objectPositionWS);
    runtimeDepth = 1 - positionCS.z / positionCS.w;
    depthSpaceScreenPosition = ComputeScreenPos(depthHCS);
}

float NormalizeWithinBounds(const float v, const float min, const float max)
{
    return clamp((v - min) / (max - min), 0, 1);
}

void SetOcclusion(float4 depthSpaceScreenPosition, float runtimeDepth, inout float4 color)
{
    const float2 uv = depthSpaceScreenPosition.xy / depthSpaceScreenPosition.w;

    if (all(uv < 0.0) || all(uv > 1.0))
    {
        return;
    }

    const float near = _ProjectionParams.y;
    const float far = _ProjectionParams.z;
    const float depthNearMin = 0.0;
    const float depthNearMax = 0.05;
    const float depthFarMin = 5.5;
    const float depthFarMax = 7.5;
    const float depthCloseToleranceThreshold = 1.5;
    const float depthFarToleranceThreshold = 5.5;
    const float toleranceClose = 0.02;
    const float toleranceFurthest = 0.5;
    const float toleranceGamma = 1;
    const float passthroughDepth = SAMPLE_TEXTURE2D_ARRAY(_PassthroughDepthTexture, sampler_PassthroughDepthTexture,
                                                           uv, unity_StereoEyeIndex).r;

    const float runtimeLinearDepth = near * far / (far - runtimeDepth * (far - near));
    const float delta = runtimeLinearDepth - passthroughDepth;

    // |____|_______|____________|_____|
    // 0  nMin   nMax         fmin   fmax
    //d                            ^

    const float trustDepthNear = NormalizeWithinBounds(passthroughDepth, depthNearMin, depthNearMax);
    const float trustDepthFar = 1 - NormalizeWithinBounds(passthroughDepth, depthFarMin, depthFarMax);
    const float sceneAssetVisibility = 1 - NormalizeWithinBounds(runtimeLinearDepth, depthFarMin, depthFarMax);
    const float tolerance_t = NormalizeWithinBounds(passthroughDepth, depthCloseToleranceThreshold, depthFarToleranceThreshold);
    const float tolerance = toleranceClose + pow(tolerance_t, toleranceGamma) * (toleranceFurthest - toleranceClose);

    //gradually change visibility 0 to 1 on depth delta values <= tolerance.
    const float closeProximityVisibility = clamp(1 - (delta + tolerance) / (2 * tolerance) * trustDepthFar, 0, 1);
    color.a *= sceneAssetVisibility * max(max(closeProximityVisibility, 0), 1 - trustDepthNear);
    color.rgb *= color.a;
}