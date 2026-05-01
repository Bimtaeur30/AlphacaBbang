Shader "Custom/FogPostProcess"
{
    Properties
    {
        _BlitTexture ("Screen",    2D) = "white" {}
        _MaskTex     ("View Mask", 2D) = "black" {}
        _FogColor    ("Fog Color", Color) = (0,0,0,0.7)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float4 _BlitScaleBias;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 scene = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, IN.uv);
                float  mask  = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uv).r;

                float4 fogged = float4(
                    scene.rgb * (1.0 - _FogColor.a) + _FogColor.rgb * _FogColor.a,
                    1.0
                );
                return lerp(fogged, scene, mask);
            }
            ENDHLSL
        }
    }
}