float2 _NdcLinearConversionParameters;

float NormalizeWithinBounds(const float v, const float min, const float max)
{
    return saturate((v - min) / (max - min));
}

//symmetric range is [-1:1] (OpenGL)
float ConvertDepthToSymmetricRange(const float depthNDC)
{
    return depthNDC * 2.0 - 1.0;
}

//non-symmetric range is [0:1] (Vulcan, DirectX, Metal)
float ConvertDepthToNonSymmetricRange(const float depthNDC)
{
    return (depthNDC + 1.0) * 0.5;
}

float LinearizeDepth(const float symmetricRangeDepthNdc)
{
    return _NdcLinearConversionParameters.x / (symmetricRangeDepthNdc + _NdcLinearConversionParameters.y);
}

float LinearDepthToSymmetricRangeNDC(const float linearDepth)
{
    return _NdcLinearConversionParameters.x / linearDepth - _NdcLinearConversionParameters.y;
}

float4 DebugDepthDistance(const float linearDepth)
{
    const float4 colors[9] =
    {
        float4(1, 1, 1, 1),         // White, 0 meter
        float4(1, 0, 0, 1),         // Red, 1 meter
        float4(1, 0.125, 0, 1),     // Orange, 2 meters
        float4(1, 1, 0, 1),         // Yellow, 3 meters
        float4(0, 1, 0, 1),         // Green, 4 meters
        float4(0.5, 0.5, 0, 1),     // Cyan, 5 meters
        float4(0, 0, 1, 1),         // Blue, 6 meters
        float4(0.375, 0.375, 1, 1), // Magenta, 7 meters
        float4(0.7, 0.25, 1, 1)     // Pink, 8 meters
    };

    float step = 1;
    float tempD = step;

    while (tempD < linearDepth && tempD < 10)
    {
        tempD += step;
    }

    int colIndex = (int) (tempD / step) - 1;
    colIndex = clamp(colIndex, 0, 7);
    return lerp(colors[colIndex], colors[colIndex + 1], (1 - (tempD - linearDepth)) / step);
}
