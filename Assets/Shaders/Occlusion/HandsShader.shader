Shader "Occlusion/HandsShader"
{
    Properties
    {
        _Passthrough ("Passthrough", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }

        ZWrite On
        ZTest LEqual
        ColorMask RGBA

        Pass
        {
            HLSLPROGRAM
            #pragma fragment frag
            #pragma vertex vert

            #include "UnityCG.cginc"

            float4 _Passthrough;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            struct appdata
            {
                float4 vertex : POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag() : SV_Target
            {
                return _Passthrough;
            }
            ENDHLSL
        }
    }
}
