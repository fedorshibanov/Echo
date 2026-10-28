Shader "Custom/Ground"
{
    Properties
    {
        [MainTexture] _TopTex ("Top Texture", 2D) = "white" {}
        _SideTex ("Side Texture", 2D) = "white" {}
        _Tiling ("Horizontal Tiling", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
            };

            TEXTURE2D(_TopTex);
            SAMPLER(sampler_TopTex);

            TEXTURE2D(_SideTex);
            SAMPLER(sampler_SideTex);

            CBUFFER_START(UnityPerMaterial)
                float _Tiling;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs pos =
                    GetVertexPositionInputs(input.positionOS.xyz);

                VertexNormalInputs norm =
                    GetVertexNormalInputs(input.normalOS);

                output.positionHCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = norm.normalWS;
                output.positionOS = input.positionOS.xyz;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 position = input.positionWS;

                float2 uv;

                if (normal.y > 0.5)
                {
                    uv = position.xz * _Tiling;

                    return SAMPLE_TEXTURE2D(
                        _TopTex,
                        sampler_TopTex,
                        uv
                    );
                }

                float horizontalUV;

                if (abs(normal.x) > abs(normal.z))
                    horizontalUV = position.z;
                else
                    horizontalUV = position.x;

                uv.x = horizontalUV * _Tiling;
                uv.y = input.positionOS.y + 0.5;

                return SAMPLE_TEXTURE2D(
                    _SideTex,
                    sampler_SideTex,
                    uv
                );
            }

            ENDHLSL
        }
    }
}