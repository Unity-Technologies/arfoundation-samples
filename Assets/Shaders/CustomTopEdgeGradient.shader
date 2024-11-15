Shader "Unlit/CustomTopEdgeGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", color) = (1, 1, 1, 1)
        _ColorB ("ColorB", color) = (1, 1, 1, 1)
        _InterpolateSpeed ("Interpolate Speed", Range(0, 2)) = 1
    }
    // URP SubShader
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.0"
        }

        Tags { "RenderType"="Transparent" "RenderQueue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
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

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float _EdgeHeight;
            float _Height;
            float _InterpolateSpeed;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                ZERO_INITIALIZE(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.localPos = v.vertex.xyz;
                return o;
            }

            float4 frag (const v2f i) : SV_Target
            {
                const float4 texCol = tex2D(_MainTex, i.uv);
                const float yVal = 1;
                float percent = ((yVal + (_Time.y * _InterpolateSpeed)) % 2) - 1;
                percent = abs(percent);
                
                float4 gradient = lerp(_ColorA, _ColorB, percent) * texCol;
                return gradient;
            }
            ENDHLSL
        }
    }
    // Built-in Render Pipeline SubShader
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderQueue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
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

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float _EdgeHeight;
            float _Height;
            float _InterpolateSpeed;

            v2f vert (appdata v)
            {
                v2f o;
 
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 

                o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.localPos = v.vertex;
                return o;
            }

            float4 frag (const v2f i) : SV_Target
            {
                const float4 texCol = tex2D(_MainTex, i.uv);
                const float yVal = 1;
                float percent = ((yVal + (_Time.y * _InterpolateSpeed)) % 2) - 1;
                percent = abs(percent);
                
                float4 gradient = lerp(_ColorA, _ColorB, percent) * texCol;
                return gradient;
            }
            ENDHLSL
        }
    }
}
