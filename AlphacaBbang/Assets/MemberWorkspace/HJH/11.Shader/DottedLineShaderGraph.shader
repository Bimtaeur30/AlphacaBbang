Shader "Custom/DottedLineShaderGraph"
{
    // 셰이더는 그냥 이거 Test니까 바꿀거면 말해줘...ㅠ
     Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DashLength ("Dash Length", Range(0.01, 0.99)) = 0.5  // 선분 길이 비율
        _DotSpacing ("Spacing", Float) = 0.3                   // 반복 간격
        _Speed ("Speed", Float) = 1.0                          // 이동 속도
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _DashLength;
                float _DotSpacing;
                float _Speed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // 점선들이 이동하는 코드
                float animU = uv.x - _Time.y * _Speed;

                // _DotSpacing 간격으로 타일링을 해주는 코드
                float cell = frac(animU / _DotSpacing);

                // 밑은 그냥 점선이 보이는 부분과 안보이는 부분을 나누는 코드
                float alpha = step(cell, _DashLength);

                return half4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}
