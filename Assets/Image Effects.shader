Shader "Hidden/Image Effects" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

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

    ENDCG

    SubShader {
        Cull Off ZTest Off ZWrite Off 

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Gamma.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Brightness.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Contrast.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Saturation.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Grain.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Blur.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Sharpness.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Effects/Toon.cginc"
            ENDCG
        }
    }
}
