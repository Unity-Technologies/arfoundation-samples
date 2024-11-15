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

    while (tempD < depth)
    {
        tempD += step;
    }

    int colIndex = (int) (tempD / step) - 1;
    colIndex = clamp(colIndex, 0, 6);
    return lerp(colors[colIndex], colors[colIndex + 1], (1 - (tempD - depth)) / step);
    //return colors[colIndex];
}

void SetOcclusionVertOutputs(float4 positionOS, inout float4 positionCS, inout float runtimeDepth, inout float4 depthSpaceScreenPosition)
{
    const float4 objectPositionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz, 1.0));
    positionCS = mul(UNITY_MATRIX_VP, objectPositionWS);
    const float4 depthHCS = mul(_DepthVPMatrices[unity_StereoEyeIndex], objectPositionWS);
    runtimeDepth = 1 - positionCS.z / positionCS.w;
    depthSpaceScreenPosition = ComputeScreenPos(depthHCS);
}

void SetOcclusion(float4 depthSpaceScreenPosition, float runtimeDepth, inout float4 color)
{
    const float2 uv = depthSpaceScreenPosition.xy / depthSpaceScreenPosition.w;

    if (all(uv < 0.0) || all(uv > 1.0))
    {
        return;
    }
    
    const float4 passthroughDepth = SAMPLE_TEXTURE2D_ARRAY(_PassthroughDepthTexture, sampler_PassthroughDepthTexture, uv, unity_StereoEyeIndex);
    const float4 confidenceBothEyes = SAMPLE_TEXTURE2D(_PassthroughConfidenceTexture, sampler_PassthroughConfidenceTexture, uv);
    const float confidenceCurrentEye = (1 - unity_StereoEyeIndex) * confidenceBothEyes.r + unity_StereoEyeIndex * confidenceBothEyes.g;
    const float near = _ProjectionParams.y;
    const float far = _ProjectionParams.z;
    const float runtimeLinearDepth = near * far / (far - runtimeDepth * (far - near));

    const float delta = runtimeLinearDepth - passthroughDepth.r;

    if(abs(delta) < 0.05)
    {
        color.a = lerp(0.5, 0, delta / 0.05) * (1 - confidenceCurrentEye);
    }
    else if(delta > 0)
    {
        color.a = 0;
    }
}
