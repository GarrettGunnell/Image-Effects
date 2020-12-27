Shader "Hidden/Saturation" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            #include "UnityCG.cginc"

            float _Saturation;

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vp(VertexData v) {
                v2f f;
                f.pos = UnityObjectToClipPos(v.vertex);
                f.uv = v.uv;

                return f;
            }

            sampler2D _MainTex;

            fixed4 fp(v2f i) : SV_TARGET {
                fixed4 col = tex2D(_MainTex, i.uv);
                float luminance = (0.2126 * col.r) +
                                  (0.71552 * col.g) +
                                  (0.0722 * col.b);

                col.rgb = lerp(luminance, col.rgb, _Saturation);

                return col;
            }

            ENDCG
        }
    }
}
