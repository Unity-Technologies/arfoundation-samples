Shader "Unlit/PlaneShaderSpecial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", color) = (1, 1, 1, 1)
        _TexColorTint ("Texture Color", color) = (1, 1, 1, 1)
    }
    // URP SubShader
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.0"
        }

        Tags { "RenderType"="Transparent" "RenderQueue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TexColorTint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 finalColor = _Color;

                float4 texCol = tex2D(_MainTex, i.uv);
                if (texCol.a != 0)
                {
                    finalColor = _TexColorTint;
                }
                return finalColor;
            }
            ENDHLSL
        }
    }
    // Built-in Render Pipeline SubShader
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderQueue"="Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TexColorTint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 finalColor = _Color;

                float4 texCol = tex2D(_MainTex, i.uv);
                if (texCol.a != 0)
                {
                    finalColor = _TexColorTint;
                }
                return finalColor;
            }
            ENDHLSL
        }
    }
}
