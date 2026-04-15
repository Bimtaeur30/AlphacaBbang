Shader "Custom/FogPostProcess"
{
    Properties
    {
        _MaskTex  ("View Mask", 2D) = "black" {}
        _FogColor ("Fog Color", Color) = (0,0,0,0.7)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            // Blitter가 자동으로 넣어주는 소스 텍스처
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _BlitTexture;  // ← _MainTex 아님
            sampler2D _MaskTex;
            float4    _FogColor;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 scene = tex2D(_BlitTexture, IN.uv);
                float  mask  = tex2D(_MaskTex, IN.uv).r;

                float4 fogged = float4(
                    scene.rgb * (1 - _FogColor.a) + _FogColor.rgb * _FogColor.a,
                    1.0
                );
                return lerp(fogged, scene, mask);
            }
            ENDCG
        }
    }
}