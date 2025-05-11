Shader "XRIT/BiRP_Fresnel"
{
    Properties
    {
        _BaseColor ("_BaseColor", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metalness", Range(0, 1)) = 0
        _RimColor ("_RimColor", Color) = (1,1,1,1)
        [PowerSlider(4)]_RimPower ("_RimPower", Range(0.25, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        #if !defined(UNITY_USES_HDRP) && !defined(UNITY_USES_URP)
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _BaseColor;
        half _Smoothness;
        half _Metallic;
        float3 _RimColor;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
              half NdotL = dot (s.Normal, lightDir);
              half4 c;
              c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
              c.a = s.Alpha;
              return c;
          }

        void surf(Input i, inout SurfaceOutputStandard o)
        {
            //sample and tint albedo texture
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            col *= _BaseColor;
            o.Albedo = col.rgb;
            //just apply the values for metalness and smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            //get the dot product between the normal and the view direction
            float fresnel = dot(i.worldNormal, i.viewDir);
            //invert the fresnel so the big values are on the outside
            fresnel = saturate(1 - fresnel);
            //raise the fresnel value to the exponents power to be able to adjust it
            fresnel = pow(fresnel, _RimPower);
            //combine the fresnel value with a color
            float3 fresnelColor = fresnel * _RimColor;
            //apply the fresnel value to the emission
            o.Emission = fresnelColor;
        }
        #endif
        ENDCG
    }
    FallBack "Diffuse"
    FallBack "Standard"
}
