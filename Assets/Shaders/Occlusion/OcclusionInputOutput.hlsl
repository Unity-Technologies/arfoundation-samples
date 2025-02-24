struct OcclusionAttributes
{
    float4 positionOS : POSITION;
};

struct OcclusionVaryings
{
    float4 positionCS : SV_POSITION;
    float4 objectPositionWS : TEXCOORD0;
};
