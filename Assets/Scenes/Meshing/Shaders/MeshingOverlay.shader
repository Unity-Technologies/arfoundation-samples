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

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct fragment_output
            {
                half4 color : SV_Target;
            };


            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Transform the position from object space to clip space.
                o.position = UnityObjectToClipPos(v.position);;
                return o;
            }


            half4 _Color;


            fragment_output frag (v2f i)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                fragment_output o;
                o.color = _Color;
                return o;
            }

            ENDHLSL
        }
    }
    Fallback Off
}
