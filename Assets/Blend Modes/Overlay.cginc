fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 blendedColor = (color < 0.5) ? 2 * color * color : 1 - 2 * (1 - color - 0.5) * (1 - color);

    return lerp(color, blendedColor, _BlendStrength);
}
