Shader "Unlit/DepthOnly"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }


        Pass
        {
            ZWrite On

            ColorMask 0
        }

    }
}
