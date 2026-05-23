Shader "Custom/FloorFog"
{
    Properties
    {   
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap   ("Base Map",   2D)    = "white" {}
        _FogColor  ("Fog Color",  Color) = (0.25, 0.25, 0.25, 0.85)
        
        [Header(Mask Blur Settings)]
        _EdgeBlurWidth ("Obstacle Mask Blur Width", Range(0.001, 0.1)) = 0.03
        _EdgeBlurSamples ("Obstacle Blur Samples", Range(4, 16)) = 8
        
        [Header(View Cone Softness)]
        _ViewSoftness ("View Distance Softness", Range(0.1, 5.0)) = 1.0
        _AngleSoftness ("View Angle Softness", Range(1.0, 20.0)) = 5.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

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
                float4 _FogColor;
                float _EdgeBlurWidth;
                float _EdgeBlurSamples;
                float _ViewSoftness;
                float _AngleSoftness;
            CBUFFER_END

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

            float GetViewMask(float3 worldPos)
            {
                float3 toTarget = worldPos - _PlayerPos.xyz;
                toTarget.y = 0;
                float dist = length(toTarget);

                float viewMask = 1.0 - smoothstep(_ViewRadius - _ViewSoftness, _ViewRadius, dist);
                float closeMask = 1.0 - smoothstep(_CloseViewRadius - _ViewSoftness, _CloseViewRadius, dist);

                if (dist > _ViewRadius) 
                    return closeMask; 

                float3 forward = normalize(_PlayerForward.xyz);
                float3 dir     = dist > 0.001 ? toTarget / dist : forward;
                float  angle   = degrees(acos(clamp(dot(forward, dir), -1.0, 1.0)));
                
                float  halfAngle = _ViewAngle * 0.5;
                float  angleMask = 1.0 - smoothstep(halfAngle - _AngleSoftness, halfAngle, angle);

                float coneMask = viewMask * angleMask;
                
                return saturate(max(coneMask, closeMask));
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
                float2 screenUV = IN.positionCS.xy / _ScreenParams.xy;

                float blurredMaskObstacle = 0;
                int   samples = (int)_EdgeBlurSamples;
                float radius  = _EdgeBlurWidth;

                for (int i = 0; i < samples; i++)
                {
                    float  angle  = (6.28318 / samples) * i;
                    float2 offset = float2(cos(angle), sin(angle)) * radius;
                    blurredMaskObstacle += SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, screenUV + offset).r;
                }
                blurredMaskObstacle /= samples;
                
                float maskRange = GetViewMask(IN.positionWS);

                float finalMask = maskRange;

                float alpha = _FogColor.a * (1.0 - finalMask);
                InputData inputData = (InputData)0;
                inputData.positionWS      = IN.positionWS;
                inputData.normalWS        = normalize(IN.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord     = TransformWorldToShadowCoord(IN.positionWS);

                float4 texColor  = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                Light  mainLight = GetMainLight(inputData.shadowCoord);
                float  NdotL     = saturate(dot(inputData.normalWS, mainLight.direction));
                float  shadow    = mainLight.shadowAttenuation;
               float3 lit = texColor.rgb * (mainLight.color * NdotL * shadow + 0.3);

                float3 fogged = lit * (1.0 - _FogColor.a) + _FogColor.rgb * _FogColor.a;
                float3 final  = lerp(fogged, lit, finalMask);
                
                float3 shadedFog = _FogColor.rgb * shadow;

                return float4(shadedFog, alpha);
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