Shader "Unlit/MeshingOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            };

            struct v2f
            {
                float4 position : SV_POSITION;
            };

            struct fragment_output
            {
                half4 color : SV_Target;
            };


            v2f vert (appdata v)
            {
                v2f o;
                // Transform the position from object space to clip space.
                o.position = UnityObjectToClipPos(v.position);;
                return o;
            }


            half4 _Color;


            fragment_output frag (v2f i)
            {
                fragment_output o;
                o.color = _Color;
                return o;
            }

            ENDHLSL
        }
    }
    Fallback Off
}
