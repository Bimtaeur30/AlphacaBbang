Shader "Custom/FogOverlay"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0.2, 0.2, 0.2, 0.85)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+2" }
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref 1
            Comp NotEqual 
            Pass Keep
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
            CBUFFER_END

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
                return _FogColor;
            }
            ENDHLSL
        }
    }
}