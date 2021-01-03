sampler2D _GrainTex;
float _Grain;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 grain = tex2D(_GrainTex, f.uv);
    fixed4 blendedColor = color * grain;

    return lerp(color, blendedColor, _Grain);
}
