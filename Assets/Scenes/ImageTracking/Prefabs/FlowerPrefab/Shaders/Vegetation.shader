Shader "Custom/Vegetation" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal", 2D)= "bump" {}
        _OcclusionMap ("Occlusion", 2D) = "white" {}
        _OcclusionStrength ("Occlusion Strength", Range(0,1)) = 0.0
        _MetallicGlossMap ("Metaillic Smoothness (R/A)", 2D) = "white" {}
        _EmissionMap ("Emissive", 2D) = "white" {}
        [hdr]_EmissiveColor ("Emissive Color", Color) = (0,0,0,0)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

        CGPROGRAM
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _OcclusionMap, _BumpMap, _MetallicGlossMap, _EmissionMap;

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Glossiness, _Metallic, _OcclusionStrength;
        fixed4 _Color, _EmissiveColor;

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) {

            #ifdef LOD_FADE_CROSSFADE
            float2 vpos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
            UnityApplyDitherCrossFade(vpos);
            #endif

            fixed4 ms = tex2D (_MetallicGlossMap, IN.uv_MainTex);

            o.Albedo =  tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_MainTex));
            o.Metallic = ms.r * _Metallic;
            o.Smoothness = ms.a * _Glossiness;
            o.Occlusion =  LerpOneTo (tex2D (_OcclusionMap, IN.uv_MainTex), _OcclusionStrength);
            o.Emission = tex2D (_EmissionMap, IN.uv_MainTex) * _EmissiveColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
