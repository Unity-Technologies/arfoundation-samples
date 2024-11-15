struct OcclusionAttributes
{
    float4 positionOS : POSITION;
};

struct OcclusionVaryings
{
    float4 positionCS : SV_POSITION;
    float runtimeDepth : TEXCOORD0;
    float4 positionDepthHCS : TEXCOORD1;
    float4 depthSpaceScreenPosition : TEXCOORD2;
};
