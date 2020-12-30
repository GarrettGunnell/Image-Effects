float _Contrast;

fixed4 fp(v2f f) : SV_TARGET {
    return tex2D(_MainTex, f.uv);
}
