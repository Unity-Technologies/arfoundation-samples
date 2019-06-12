Shader "Unlit/ARKitLWRP"
{
    Properties
    {
        _textureY ("TextureY", 2D) = "white" {}
        _textureCbCr ("TextureCbCr", 2D) = "black" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline"}
        LOD 100

        Pass
        {
            Name "Default"
            Tags { "LightMode" = "LightweightForward"}

            ZTest Always ZWrite Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"

            float4x4 _UnityDisplayTransform;

            struct VertexInput
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct VertexOutput
            {
                half4 pos       : SV_POSITION;
                half2 uv        : TEXCOORD0;
            };


            TEXTURE2D(_textureY);
            SAMPLER(sampler_textureY);
            TEXTURE2D(_textureCbCr);
            SAMPLER(sampler_textureCbCr);

            VertexOutput Vertex(VertexInput i)
            {
                VertexOutput o;
                o.pos = TransformObjectToHClip(i.vertex.xyz);
                o.uv.x = (_UnityDisplayTransform[0].x * i.uv.x + _UnityDisplayTransform[1].x * (i.uv.y) + _UnityDisplayTransform[2].x);
                o.uv.y = (_UnityDisplayTransform[0].y * i.uv.x + _UnityDisplayTransform[1].y * (i.uv.y) + _UnityDisplayTransform[2].y);
                return o;
            }

            half4 Fragment(VertexOutput i) : SV_Target
            {
                half y = SAMPLE_TEXTURE2D(_textureY, sampler_textureY, i.uv).r;
                half4 ycbcr = half4(y, SAMPLE_TEXTURE2D(_textureCbCr, sampler_textureCbCr, i.uv).rg, 1.0);

                const half4x4 ycbcrToRGBTransform = half4x4(
                     half4(1.0, +0.0000, +1.4020, -0.7010),
                     half4(1.0, -0.3441, -0.7141, +0.5291),
                     half4(1.0, +1.7720, +0.0000, -0.8860),
                    half4(0.0, +0.0000, +0.0000, +1.0000)
                 );

                return mul(ycbcrToRGBTransform, ycbcr);
             }
            ENDHLSL
        }
    }

}
