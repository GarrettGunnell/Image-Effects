fixed4 fp(v2f f) : SV_TARGET {
    fixed4 base = tex2D(_MainTex, f.uv);
    fixed4 blend = tex2D(_MainTex, f.uv);
    fixed4 blendedColor = 1 - (1 - base) / blend;

    return lerp(base, blendedColor, _BlendStrength);
}
