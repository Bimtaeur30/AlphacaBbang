Shader "Custom/FovMask"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        ZWrite Off
        ZTest Always
        ColorMask 0  

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return float4(0,0,0,0); 
            }
            ENDHLSL
        }
    }
}