fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 blendedColor = (1 - 2 * color) * (color * color) + (2 * color * color);

    return lerp(color, blendedColor, _BlendStrength);
}
