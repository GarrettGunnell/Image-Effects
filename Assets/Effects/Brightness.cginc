float _Brightness;

fixed4 fp(v2f i) : SV_TARGET {
    return tex2D(_MainTex, i.uv) * _Brightness;
}
