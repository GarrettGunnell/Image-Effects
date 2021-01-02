Shader "Hidden/Blend Modes" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed _BlendStrength;

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

                fixed4 fp(v2f f) : SV_TARGET {
                    return tex2D(_MainTex, f.uv);
                }
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Multiply.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Screen.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Soft Light.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Color Burn.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Color Dodge.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Linear Burn.cginc"
            ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                #include "Blend Modes/Linear Dodge.cginc"
            ENDCG
        }
    }
}
