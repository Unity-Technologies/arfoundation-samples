Shader "Universal Render Pipeline/SobelEdge"
{
    // This shader is only compatible in URP version 12.0 or higher
    Properties
    {
        _MainTex("Input Texture", 2D) = "white" {}
        // Meta's camera resolution 1s 1280 x 960
        _TexelSize("Texel Size (1/width,1/height)", Vector) = (0.00078125, 0.00104167, 0, 0)
        _EdgeStrength("Edge Strength", Range(0, 5)) = 1
        _Threshold("Threshold", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Name "SobelPass"
            Tags { "LightMode"="UniversalForward" } // Works as an unlit pass in URP

            Cull Off
            ZWrite Off
            ZTest Always
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv          : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _TexelSize;     // x = 1/width, y = 1/height
            float  _EdgeStrength;
            float  _Threshold;

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // Convert color to luminance (perceptual)
            float Luma(float3 c)
            {
                // Rec. 709 weights
                return dot(c, float3(0.2126, 0.7152, 0.0722));
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Sample 3x3 neighborhood
                float2 dx = float2(_TexelSize.x, 0);
                float2 dy = float2(0, _TexelSize.y);

                float tl = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - dx - dy).rgb);
                float  t = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv        - dy).rgb);
                float tr = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + dx - dy).rgb);

                float l  = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - dx       ).rgb);
                float c  = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv            ).rgb);
                float r  = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + dx       ).rgb);

                float bl = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - dx + dy).rgb);
                float  b = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv        + dy).rgb);
                float br = Luma(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + dx + dy).rgb);

                // Sobel kernels
                float gx = (-tl - 2.0 * l - bl) + (tr + 2.0 * r + br);
                float gy = (-tl - 2.0 * t - tr) + (bl + 2.0 * b + br);

                float g = sqrt(gx * gx + gy * gy) * _EdgeStrength;

                // Optional thresholding
                if (_Threshold > 0)
                {
                    g = g > _Threshold ? 1.0 : 0.0;
                }

                return float4(g, g, g, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
