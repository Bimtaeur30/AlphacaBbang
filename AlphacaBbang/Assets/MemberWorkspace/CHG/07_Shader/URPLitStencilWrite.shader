Shader "Custom/URPLitStencilWrite"
{
    Properties
    {
        // Base Map
        _BaseMap("Albedo", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)

        // Metallic / Smoothness
        _Metallic("Metallic", Range(0,1)) = 0
        _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0
        _Smoothness("Smoothness", Range(0,1)) = 0.5

        // Normal Map
        [Toggle(_NORMALMAP)] _EnableNormalMap("Enable Normal Map", Float) = 0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0

        // Occlusion
        _OcclusionStrength("Occlusion Strength", Range(0,1)) = 1.0
        _OcclusionMap("Occlusion Map", 2D) = "white" {}

        // Emission
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0)
        _EmissionMap("Emission Map", 2D) = "black" {}

        // Stencil
        [IntRange] _StencilRef("Stencil Reference", Range(0,255)) = 2
        [IntRange] _StencilWriteMask("Stencil Write Mask", Range(0,255)) = 255
        [IntRange] _StencilReadMask("Stencil Read Mask", Range(0,255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Always
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass("Stencil Pass", Float) = 2 // Replace
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail", Float) = 0 // Keep
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail("Stencil ZFail", Float) = 0 // Keep

        // Render State
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2 // Back
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Stencil
        {
            Ref [_StencilRef]
            WriteMask [_StencilWriteMask]
            ReadMask [_StencilReadMask]
            Comp [_StencilComp]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        Cull [_Cull]
        ZWrite On
        ZTest LEqual

        // ------------------------------------------------------------------
        // Forward Lit Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma shader_feature _NORMALMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);        SAMPLER(sampler_BumpMap);
            TEXTURE2D(_OcclusionMap);   SAMPLER(sampler_OcclusionMap);
            TEXTURE2D(_EmissionMap);    SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half   _Metallic;
                half   _Smoothness;
                half   _BumpScale;
                half   _OcclusionStrength;
                half4  _EmissionColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float4 tangentWS   : TEXCOORD3;
                float4 fogFactorAndVertexLight : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   nrmInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                OUT.positionHCS = posInputs.positionCS;
                OUT.positionWS  = posInputs.positionWS;
                OUT.uv          = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS    = nrmInputs.normalWS;
                OUT.tangentWS   = float4(nrmInputs.tangentWS, IN.tangentOS.w * GetOddNegativeScale());

                half fogFactor = ComputeFogFactor(posInputs.positionCS.z);
                half3 vertexLight = VertexLighting(posInputs.positionWS, nrmInputs.normalWS);
                OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 baseMap   = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 baseColor = baseMap * _BaseColor;

                // Normal
                float3 normalWS = normalize(IN.normalWS);
                #ifdef _NORMALMAP
                    half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv);
                    half3 normalTS  = UnpackNormalScale(normalMap, _BumpScale);
                    float3 bitangent = IN.tangentWS.w * cross(IN.normalWS, IN.tangentWS.xyz);
                    normalWS = TransformTangentToWorld(normalTS,
                        half3x3(IN.tangentWS.xyz, bitangent, IN.normalWS));
                    normalWS = normalize(normalWS);
                #endif

                // Occlusion
                half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, IN.uv).g;
                occ = LerpWhiteTo(occ, _OcclusionStrength);

                // PBR Surface
                SurfaceData surface = (SurfaceData)0;
                surface.albedo      = baseColor.rgb;
                surface.alpha       = baseColor.a;
                surface.metallic    = _Metallic;
                surface.smoothness  = _Smoothness;
                surface.normalTS    = half3(0,0,1);
                surface.occlusion   = occ;
                surface.emission    = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.uv).rgb * _EmissionColor.rgb;

                InputData inputData = (InputData)0;
                inputData.positionWS        = IN.positionWS;
                inputData.normalWS          = normalWS;
                inputData.viewDirectionWS   = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord       = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord          = IN.fogFactorAndVertexLight.x;
                inputData.vertexLighting    = IN.fogFactorAndVertexLight.yzw;
                inputData.bakedGI           = SAMPLE_GI(IN.uv, inputData.vertexLighting, normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionHCS);
                inputData.shadowMask        = SAMPLE_SHADOWMASK(IN.uv);

                half4 color = UniversalFragmentPBR(inputData, surface);
                color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);

                return color;
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
