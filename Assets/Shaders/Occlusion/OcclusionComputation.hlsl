#include "Utils.hlsl"

TEXTURE2D(_EnvironmentConfidenceTexture);
SAMPLER(sampler_EnvironmentConfidenceTexture);

TEXTURE2D_ARRAY_FLOAT(_EnvironmentDepthTexture);
SAMPLER(sampler_EnvironmentDepthTexture);
float2 _EnvironmentDepthTexture_TexelSize;

TEXTURE2D_ARRAY_FLOAT(_EnvironmentDepthTexturePreprocessed);
SAMPLER(sampler_EnvironmentDepthTexturePreprocessed);
float4 _EnvironmentDepthTexturePreprocessed_TexelSize;

float4x4 _EnvironmentDepthProjectionMatrices[2];

void SetOcclusionVertOutputs(float4 positionOS, inout float4 positionCS, inout float4 objectPositionWS)
{
    objectPositionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz, 1.0));
    positionCS = mul(UNITY_MATRIX_VP, objectPositionWS);
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
        return LinearizeDepth(ConvertDepthToSymmetricRange(environmentDepth));
    #endif
}

float4 SamplePreprocessedEnvironmentDepth(const float2 uv, const float index)
{
    return _EnvironmentDepthTexturePreprocessed.Sample(sampler_EnvironmentDepthTexturePreprocessed, float3(uv, index));
}

float4 SampleEnvironmentDepthGeneral(const float2 uv)
{
    #ifdef XR_SOFT_OCCLUSION
        return SamplePreprocessedEnvironmentDepth(uv, unity_StereoEyeIndex);
    #else
        return float4(SampleEnvironmentDepthLinear(uv), 0, 0, 0);
    #endif
}

float GetHardPixelVisibility(const float linearEnvironmentDepth, const float linearSceneDepth)
{
    return float (linearEnvironmentDepth > linearSceneDepth);
}

float GetSoftPixelVisibility(const float4 depthSample, const float linearSceneDepth)
{
    const float symmetricSceneDepthNDC = LinearDepthToSymmetricRangeNDC(linearSceneDepth);
    const float nonSymmetricSceneDepthNDC = ConvertDepthToNonSymmetricRange(symmetricSceneDepthNDC);

    // inversed scale factor is inversely proportional to (nonSymmetricSceneDepthNDC - 1.0f)
    // at ndc value == 0 formula must return scaleAtZero scale factor
    const float scaleAtZero = 15.0f; // determined experimentally
    float invScaleFactor = -scaleAtZero / (nonSymmetricSceneDepthNDC - 1.0f);

    float3 minMaxMidEnvDepths = float3(1.0f - depthSample.x, 1.0f - depthSample.y, depthSample.z + 1.0f - depthSample.x);

    float3 depthDeltas = saturate((minMaxMidEnvDepths - nonSymmetricSceneDepthNDC) * invScaleFactor);

    // blend min and max deltas
    // min delta -> object fully invisible
    // max delta -> object fully visible
    const float kForegroundLevel = 0.1f;
    const float kBackgroundLevel = 0.9f;
    const float interp = depthSample.z / depthSample.w;
    const float blendFactor = smoothstep(kForegroundLevel, kBackgroundLevel, interp);
    const float differenceThreshold = 0.05f ;
    const float isBlending = step(differenceThreshold, depthDeltas.y - depthDeltas.x);
    const float alpha = depthDeltas.z * (1 - isBlending) + lerp(depthDeltas.x, depthDeltas.y, blendFactor) * isBlending;

    return alpha;
}

float GetToleranceBasedPixelVisibility(const float linearEnvironmentDepth, const float linearSceneDepth)
{
    const float depthNearMin = 0.0;
    const float depthNearMax = 0.05;
    const float depthFarMin = 4.5;
    const float depthFarMax = 6.5;
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
    const float closeProximityVisibility = saturate(1 - (delta + tolerance) / (2 * tolerance) * trustDepthFar);
    return sceneAssetVisibility * max(max(closeProximityVisibility, 0), 1 - trustDepthNear);
}

float ComputePixelVisibility(const float4 environmentDepth, const float linearSceneDepth)
{
    #ifdef XR_SOFT_OCCLUSION
        return GetSoftPixelVisibility(environmentDepth, linearSceneDepth);
    #elif XR_HARD_OCCLUSION
        return GetHardPixelVisibility(environmentDepth.x, linearSceneDepth);
    #else
        return GetToleranceBasedPixelVisibility(environmentDepth.x, linearSceneDepth);
    #endif

    return 1;
}

void SetOcclusion(float4 objectPositionWS, inout float4 color)
{
    const float4 clipSpaceDepthRelativePos = mul(_EnvironmentDepthProjectionMatrices[unity_StereoEyeIndex], objectPositionWS);
    const float2 uv = (clipSpaceDepthRelativePos.xy / clipSpaceDepthRelativePos.w + 1.0f) * 0.5f;

    if (all(uv < 0.0) || all(uv > 1.0))
    {
        return;
    }

    const float4 environmentDepth = SampleEnvironmentDepthGeneral(uv);
    const float sceneDepthNDC = clipSpaceDepthRelativePos.z / clipSpaceDepthRelativePos.w;
    const float linearSceneDepth = LinearizeDepth(sceneDepthNDC);

    color.a *= ComputePixelVisibility(environmentDepth, linearSceneDepth);
    color.rgb *= color.a;
}
