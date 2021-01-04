fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);

    color.r *= color.r;
    color.g *= smoothstep(0, 1, color.g);
    color.b *= color.b;

    return color;
}