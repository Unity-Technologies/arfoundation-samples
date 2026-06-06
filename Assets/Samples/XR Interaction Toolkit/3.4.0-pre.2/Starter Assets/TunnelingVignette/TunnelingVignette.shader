Shader "VR/TunnelingVignette"
{
    Properties
    {
        _ApertureSize("Aperture Size", Range(0, 1)) = 0.7
        _FeatheringEffect("Feathering Effect", Range(0, 1)) = 0.2
        _VignetteColor("Vignette Color", Color) = (0, 0, 0, 1)
        _VignetteColorBlend("Vignette Color Blend", Color) = (0, 0, 0, 1)
    }
        SubShader
    {
        Tags { "Queue" = "Transparent+5" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _VignetteColor;
            float4 _VignetteColorBlend;
            float _ApertureSize;
            float _FeatheringEffect;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float uvY = i.uv.y;
                float alphaMin = (0.5 - sqrt(0.25 - ((_ApertureSize * _ApertureSize) * 0.25)));
                float alpha = saturate(((uvY - alphaMin) / (_FeatheringEffect * _FeatheringEffect + 0.0001)));
                fixed4 color = lerp(_VignetteColor, _VignetteColorBlend, uvY * 2);
                color.w *= alpha;

                return color;
            }
            ENDCG
        }
    }
}
