Shader "Unlit/HumanStencil"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
        _TextureRotation ("Texture Rotation (Degrees)", Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "ForceNoShadowCasting" = "True"
        }

        Pass
        {
            Cull Off
            ZTest Always
            ZWrite Off
            Lighting Off
            LOD 100
            Tags
            {
                "LightMode" = "Always"
            }


            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define real half
            #define real3 half3
            #define real4 half4
            #define TransformObjectToHClip UnityObjectToClipPos

            #define DECLARE_TEXTURE2D_FLOAT(texture) UNITY_DECLARE_TEX2D_FLOAT(texture)
            #define DECLARE_SAMPLER_FLOAT(sampler)
            #define SAMPLE_TEXTURE2D(texture,sampler,texcoord) UNITY_SAMPLE_TEX2D(texture,texcoord)


            struct appdata
            {
                float3 position : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct fragment_output
            {
                real4 color : SV_Target;
            };


            real _TextureRotation;

            v2f vert (appdata v)
            {
                float angle = radians(_TextureRotation);
                float cosrot = cos(angle);
                float sinrot = sin(angle);

                float3x3 textureXformMatrix = float3x3(
                    float3(cosrot, -sinrot, 0.5f),
                    float3(sinrot, cosrot, 0.5f),
                    float3(0.0f, 0.0f, 1.0f)
                );

                float3 tmp = float3(v.texcoord.x - 0.5f, 0.5f - v.texcoord.y, 1.0f);

                v2f o;
                o.position = TransformObjectToHClip(v.position);
                o.texcoord = mul(textureXformMatrix, tmp).xy;
                return o;
            }


            DECLARE_TEXTURE2D_FLOAT(_MainTex);
            DECLARE_SAMPLER_FLOAT(sampler_MainTex);

            fragment_output frag (v2f i)
            {
                float stencilValue = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).r;

                fragment_output o;
                o.color = real4(stencilValue, stencilValue, stencilValue, 1.0h);
                return o;
            }

            ENDHLSL
        }
    }
}
