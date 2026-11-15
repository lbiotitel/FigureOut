Shader "Custom/TerrainTree_VisionClipper"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _MainTex ("Tree Texture", 2D) = "white" {}
        _HoleRadius ("Hole Radius", Float) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            sampler2D _MainTex;
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BaseColor;
                float _HoleRadius;
            CBUFFER_END


            float3 VisionTargetPos;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                
                // UV 타일링 및 오프셋 적용 (_MainTex 사용)
                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 텍스처 색상 추출 (_MainTex 사용)
                half4 col = tex2D(_MainTex, input.uv) * _BaseColor;

                // 설정한 VisionTargetPos와 거리 계산!
                float dist = distance(input.positionWS.xz, VisionTargetPos.xz);

                // 픽셀 파괴 (구멍 뚫기)
                clip(dist - _HoleRadius);
                clip(col.a - 0.1); 

                return col;
            }
            ENDHLSL
        }
    }
}