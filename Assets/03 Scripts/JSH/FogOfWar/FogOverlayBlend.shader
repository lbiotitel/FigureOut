// ============================================================================
// FogOverlayBlend.shader
// 역할: 맵 전체를 덮는 FogPlane(Quad)에 입히는 셰이더
//       RT_FogMask를 읽어 시야 내부는 투명, 외부는 안개색으로 칠함
//
// [작동 원리]
//   - RT_FogMask의 R채널 값:
//     1.0 (흰색) = 시야 안 → 안개 없음 (alpha = 0, 투명)
//     0.0 (검정) = 시야 밖 → 안개 있음 (alpha = fogColor.a, 불투명)
//   - Bilinear 필터링으로 경계가 부드럽게 번짐
//
// [FogPlane 오브젝트 설정]
//   Transform: 맵 전체를 덮을 크기로 Scale 조정 (예: Scale = 100, 1, 100)
//   Position Y: 플레이어보다 약간 높게 (예: Y = 1)
//   Material: 이 셰이더로 만든 Material 사용
//   Layer: Default (FogCamera는 VisionLayer만 찍으므로 이 오브젝트는 찍히지 않음)
// ============================================================================
// Shader "Custom/FogOverlayBlend"
// {
//     Properties
//     {
//         _FogMask  ("Fog Mask (RT)", 2D) = "black" {}
//         _FogColor ("Fog Color",  Color) = (0, 0, 0, 0.85)
//     }

//     SubShader
//     {
//         Tags
//         {
//             "RenderType"     = "Transparent"
//             "RenderPipeline" = "UniversalPipeline"
//             "Queue"          = "Transparent+100"
//         }

//         Pass
//         {
//             ZWrite Off
//             ZTest  Always
//             Cull   Off
//             Blend  SrcAlpha OneMinusSrcAlpha

//             HLSLPROGRAM
//             #pragma vertex   vert
//             #pragma fragment frag
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//             TEXTURE2D(_FogMask);
//             SAMPLER(sampler_FogMask);

//             CBUFFER_START(UnityPerMaterial)
//                 float4 _FogMask_ST;
//                 half4  _FogColor;
//             CBUFFER_END

//             struct Attributes
//             {
//                 float4 positionOS : POSITION;
//                 float2 uv         : TEXCOORD0;
//             };

//             struct Varyings
//             {
//                 float4 positionCS : SV_POSITION;
//                 float2 uv         : TEXCOORD0;
//             };

//             Varyings vert(Attributes IN)
//             {
//                 Varyings OUT;
//                 OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
//                 //OUT.uv = TRANSFORM_TEX(IN.uv, _FogMask);
//                 OUT.uv = IN.uv;
//                 return OUT;
//             }

//             half4 frag(Varyings IN) : SV_Target
//             {
//                 float visibility = SAMPLE_TEXTURE2D(_FogMask, sampler_FogMask, IN.uv).r;
//                 float alpha = _FogColor.a * saturate(1.0 - visibility);
//                 return half4(_FogColor.rgb, alpha);
//             }
//             ENDHLSL
//         }
//     }
// }
Shader "Custom/FogOverlayBlend"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0, 0, 0, 0.85)
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent+100"
        }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 머티리얼을 거치지 않고 글로벌 변수로 직접 받아옵니다!
            TEXTURE2D(_GlobalFogMask);
            SAMPLER(sampler_GlobalFogMask);

            CBUFFER_START(UnityPerMaterial)
                half4 _FogColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0; // 월드 좌표계를 가져옵니다
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = vertexInput.positionCS;
                OUT.positionWS = vertexInput.positionWS;
                return OUT;
            }

            float4 _FogCamParams;
            half4 frag(Varyings IN) : SV_Target
            {
                float camX = _FogCamParams.x;
                float camZ = _FogCamParams.y;
                float orthoSize = _FogCamParams.z;
                float size2x = orthoSize * 2.0;

                //맵의 크기나 위치가 바뀌어도 완벽하게 UV(0~1)를 맵핑하는 마법의 공식!
                float2 screenUV = float2(
                    (IN.positionWS.x - camX + orthoSize) / size2x,
                    (IN.positionWS.z - camZ + orthoSize) / size2x
                );

                float visibility = SAMPLE_TEXTURE2D(_GlobalFogMask, sampler_GlobalFogMask, screenUV).r;
                float alpha = _FogColor.a * saturate(1.0 - visibility);

                return half4(_FogColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}