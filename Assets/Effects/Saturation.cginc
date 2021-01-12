float _Saturation;
float _Gamma;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 gammaCorrected = pow(color, 1 / _Gamma);
    float luminance = (0.2126 * gammaCorrected.r) +
                      (0.71552 * gammaCorrected.g) +
                      (0.0722 * gammaCorrected.b);

    color.rgb = lerp(luminance, color.rgb, _Saturation);

    return color;
}
