Shader "Unlit/EnvironmentDepthGradient"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
        _MinDistance ("Min Distance", Float) = 0.0
        _MaxDistance ("Max Distance", Float) = 8.0
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

                float2 tmp = float2(0.5f - v.texcoord.x, v.texcoord.y - 0.5f);

                v2f o;
                o.position = TransformObjectToHClip(v.position);
                o.texcoord = float2(tmp.x * cosrot - tmp.y * sinrot + 0.5f, tmp.x * sinrot + tmp.y * cosrot + 0.5f);
                return o;
            }


            real3 HSVtoRGB(real3 arg1)
            {
                real4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                real3 P = abs(frac(arg1.xxx + K.xyz) * 6.0 - K.www);
                return arg1.z * lerp(K.xxx, saturate(P - K.xxx), arg1.y);
            }


            DECLARE_TEXTURE2D_FLOAT(_MainTex);
            DECLARE_SAMPLER_FLOAT(sampler_MainTex);

            real _MinDistance;
            real _MaxDistance;

            fragment_output frag (v2f i)
            {
                // Sample the environment depth (in meters).
                float envDistance = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).r;

                real lerpFactor = (envDistance - _MinDistance) / (_MaxDistance - _MinDistance);
                real hue = lerp(0.70f, -0.15f, saturate(lerpFactor));
                if (hue < 0.0f)
                {
                    hue += 1.0f;
                }
                real3 color = real3(hue, 0.9, 0.6);

                fragment_output o;
                o.color = real4(HSVtoRGB(color), 1.0);
                return o;
            }

            ENDHLSL
        }
    }
}
