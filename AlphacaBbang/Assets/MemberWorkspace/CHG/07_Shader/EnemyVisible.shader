Shader "Custom/EnemyVisible"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap   ("Base Map",   2D)    = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END

            // 글로벌 변수
            float4 _PlayerPos;
            float4 _PlayerForward;
            float  _ViewRadius;
            float  _ViewAngle;
            float  _CloseViewRadius;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            bool IsInViewRange(float3 worldPos)
            {
                float3 toTarget = worldPos - _PlayerPos.xyz;
                toTarget.y = 0;
                float dist = length(toTarget);

                if (dist <= _CloseViewRadius)
                    return true;

                if (dist <= _ViewRadius)
                {
                    float3 forward = normalize(_PlayerForward.xyz);
                    float3 dir     = normalize(toTarget);
                    float  angle   = degrees(acos(clamp(dot(forward, dir), -1.0, 1.0)));
                    if (angle <= _ViewAngle * 0.5)
                        return true;
                }
                return false;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.uv         = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 screenUV     = IN.positionCS.xy / _ScreenParams.xy;
                float  maskObstacle = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, screenUV).r;

                float maskRange = IsInViewRange(IN.positionWS) ? 1.0 : 0.0;

                clip(maskObstacle * maskRange - 0.5);

                InputData inputData = (InputData)0;
                inputData.positionWS      = IN.positionWS;
                inputData.normalWS        = normalize(IN.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord     = TransformWorldToShadowCoord(IN.positionWS);

                Light  mainLight = GetMainLight(inputData.shadowCoord);
                float  NdotL     = saturate(dot(inputData.normalWS, mainLight.direction));
                float  shadow    = mainLight.shadowAttenuation;

                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float3 color    = texColor.rgb * _BaseColor.rgb * (mainLight.color * NdotL * shadow + 0.3);
                return float4(color, 1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;
            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS    = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionCS  = TransformWorldToHClip(ApplyShadowBias(posWS, normalWS, _LightDirection));
                return OUT;
            }
            float4 frag(Varyings IN) : SV_Target { return 0; }
            ENDHLSL
        }
    }
}   