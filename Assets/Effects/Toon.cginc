float _LuminanceThreshold;
int _Averaging;
float _Gamma;
sampler2D _AverageColorTex;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 gammaAdjusted = pow(color, 1 / _Gamma);
        float luminance = (0.2126 * gammaAdjusted.r) +
                      (0.71552 * gammaAdjusted.g) +
                      (0.0722 * gammaAdjusted.b);

    if (_Averaging) {
        fixed4 average = tex2D(_AverageColorTex, f.uv);
        return luminance < _LuminanceThreshold ? 0 : average;
    }
    
    return luminance < _LuminanceThreshold ? 0 : color;
}
