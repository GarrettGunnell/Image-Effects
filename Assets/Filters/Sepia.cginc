fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 sepia = 0;

    sepia.r = 0.393 * color.r + 0.769 * color.g + 0.189 * color.b;
    sepia.g = 0.349 * color.r + 0.686 * color.g + 0.168 * color.b;
    sepia.b = 0.272 * color.r + 0.534 * color.g + 0.131 * color.b;

    return sepia;
}