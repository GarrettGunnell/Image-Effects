float _Gamma;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);

    return pow(color, _Gamma);
}
