float _Saturation;

fixed4 fp(v2f i) : SV_TARGET {
    fixed4 col = tex2D(_MainTex, i.uv);
    float luminance = (0.2126 * col.r) +
                      (0.71552 * col.g) +
                      (0.0722 * col.b);

    col.rgb = lerp(luminance, col.rgb, _Saturation);

    return col;
}
