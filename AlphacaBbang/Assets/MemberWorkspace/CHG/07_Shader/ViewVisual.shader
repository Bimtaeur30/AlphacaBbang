Shader "Custom/ViewVisual"
{
    Properties
    {
        _Color ("View Color", Color) = (1, 1, 1, 0.15)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
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
                return _Color;
            }
            ENDHLSL
        }
    }
}