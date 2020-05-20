Shader "Unlit/NormalOverlay"
{
    Properties
    {
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Tags
        {
            "Queue" = "AlphaTest"
            "RenderType" = "Transparent"
            "ForceNoShadowCasting" = "True"
        }

        Pass
        {
            ZTest LEqual
            ZWrite On
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

            struct appdata
            {
                float3 position : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                half3 worldNormal : TEXCOORD0;
            };

            struct fragment_output
            {
                half4 color : SV_Target;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }


            float _Transparency;

            fragment_output frag (v2f i)
            {
                fragment_output o;
                o.color.xyz = i.worldNormal * 0.5f + 0.5f;
                o.color.w = _Transparency;
                return o;
            }

            ENDHLSL
        }
    }
    Fallback Off
}
