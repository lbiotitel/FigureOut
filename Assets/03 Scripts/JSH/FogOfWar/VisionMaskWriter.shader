// ============================================================================
// VisionMaskWriter.shader
// 역할: VisionMeshGenerator가 만든 시야 메쉬에 입히는 셰이더
//       조명/그림자 연산 없이 무조건 순백색(1.0)만 출력
//
// [작동 원리]
//   FogCamera(VisionLayer만 찍음)가 이 셰이더를 렌더링하면
//   RT_FogMask 텍스처에 시야 영역만 하얗게 기록됨
//
// [VisionMeshObject 설정]
//   GameObject의 Layer → "VisionLayer" (Inspector에서 새 레이어 추가 필요)
//   MeshRenderer의 Material → 이 셰이더로 만든 Material 사용
// ============================================================================

Shader "Hidden/VisionMaskWriter"
{
    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }

        Pass
        {
            ZWrite Off
            ZTest  Always
            Cull   Off
            Blend  One Zero

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(1, 1, 1, 1); // 시야 영역 = 순백색
            }
            ENDHLSL
        }
    }
}