Shader "Custom/UIImage"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}

        // These are here to satisfy console warnings
        [HideInInspector]
        _Stencil ("Stencil", Float) = 1
        [HideInInspector]
        _StencilOp ("Stencil Op", Float) = 1
        [HideInInspector]
        _StencilComp ("Stencil Comp", Float) = 1
        [HideInInspector]
        _StencilReadMask ("Stencil Read Mask", Float) = 1
        [HideInInspector]
        _StencilWriteMask ("Stencil Write Mask", Float) = 1
        [HideInInspector]
        _ColorMask ("ColorMask", Float) = 1
    }
    // URP SubShader
    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "12.0"
        }

        Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off

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
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                ZERO_INITIALIZE(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color;
            }
            ENDHLSL
        }
    }
    // Built-in Render Pipeline SubShader
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off

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
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color;
            }
            ENDHLSL
        }
    }
}
