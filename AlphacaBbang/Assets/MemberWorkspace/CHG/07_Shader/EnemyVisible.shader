Shader "Custom/EnemyVisible"
{
    Properties
    {
        _BaseColor          ("Base Color",            Color)   = (1,1,1,1)
        _BaseMap            ("Base Map",               2D)     = "white" {}
        _MetallicGlossMap   ("Metallic",               2D)     = "white" {}
        _Metallic           ("Metallic",            Range(0,1)) = 0
        _Smoothness         ("Smoothness",          Range(0,1)) = 0.5
        _BumpMap            ("Normal Map",             2D)     = "bump" {}
        _OcclusionMap       ("Occlusion",              2D)     = "white" {}
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
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);         SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MetallicGlossMap);SAMPLER(sampler_MetallicGlossMap);
            TEXTURE2D(_BumpMap);         SAMPLER(sampler_BumpMap);
            TEXTURE2D(_OcclusionMap);    SAMPLER(sampler_OcclusionMap);
            TEXTURE2D(_MaskTex);         SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float  _Metallic;
                float  _Smoothness;
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
                float4 tangentOS  : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 positionWS  : TEXCOORD2;
                float3 tangentWS   : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };

            bool IsInViewRange(float3 worldPos)
            {
                float3 toTarget = worldPos - _PlayerPos.xyz;
                toTarget.y = 0;
                float dist = length(toTarget);
                if (dist <= _CloseViewRadius) return true;
                if (dist <= _ViewRadius)
                {
                    float3 forward = normalize(_PlayerForward.xyz);
                    float3 dir     = normalize(toTarget);
                    float  angle   = degrees(acos(clamp(dot(forward, dir), -1.0, 1.0)));
                    if (angle <= _ViewAngle * 0.5) return true;
                }
                return false;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   norInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.positionCS  = posInputs.positionCS;
                OUT.positionWS  = posInputs.positionWS;
                OUT.uv          = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS    = norInputs.normalWS;
                OUT.tangentWS   = norInputs.tangentWS;
                OUT.bitangentWS = norInputs.bitangentWS;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // 시야 체크
                float2 screenUV     = IN.positionCS.xy / _ScreenParams.xy;
                float  maskObstacle = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, screenUV).r;
                float  maskRange    = IsInViewRange(IN.positionWS) ? 1.0 : 0.0;
                clip(maskObstacle * maskRange - 0.5);

                // Lit 라이팅
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                float3 normalTS  = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv));
                float3 normalWS  = normalize(
                    normalTS.x * IN.tangentWS +
                    normalTS.y * IN.bitangentWS +
                    normalTS.z * IN.normalWS
                );

                float4 metallicGloss = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, IN.uv);
                float  metallic      = metallicGloss.r * _Metallic;
                float  smoothness    = metallicGloss.a * _Smoothness;
                float  occlusion     = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, IN.uv).r;

                InputData inputData = (InputData)0;
                inputData.positionWS      = IN.positionWS;
                inputData.normalWS        = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord     = TransformWorldToShadowCoord(IN.positionWS);
                inputData.bakedGI         = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV = screenUV;

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo       = baseColor.rgb;
                surfaceData.alpha        = baseColor.a;
                surfaceData.metallic     = metallic;
                surfaceData.smoothness   = smoothness;
                surfaceData.occlusion    = occlusion;
                surfaceData.normalTS     = normalTS;

                return UniversalFragmentPBR(inputData, surfaceData);
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