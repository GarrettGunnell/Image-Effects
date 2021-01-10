float _LuminanceThreshold;
int _Averaging;
sampler2D _AverageColorTex;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    float luminance = (0.2126 * color.r) +
                      (0.71552 * color.g) +
                      (0.0722 * color.b);

    if (_Averaging) {
        fixed4 average = tex2D(_AverageColorTex, f.uv);
        return luminance < _LuminanceThreshold ? 0 : average;
    }
    
    return luminance < _LuminanceThreshold ? 0 : color;
}
