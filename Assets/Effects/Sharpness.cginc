float _Sharpness;
sampler2D _BlurredTex;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 original = tex2D(_MainTex, f.uv);
    fixed4 blurred = tex2D(_BlurredTex, f.uv);

    return original + (original - blurred) * _Sharpness;   
}
