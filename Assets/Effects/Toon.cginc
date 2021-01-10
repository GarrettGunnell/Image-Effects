float _LuminanceThreshold;
int _Averaging;
sampler2D _AverageColorTex;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    float luminance = color.r + color.g + color.b / 3;

    if (_Averaging) {
        fixed4 average = tex2D(_AverageColorTex, f.uv);
        return luminance < _LuminanceThreshold ? 0 : average;
    }
    
    return luminance < _LuminanceThreshold ? 0 : color;
}
