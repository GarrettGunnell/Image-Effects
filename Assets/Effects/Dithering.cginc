float _LuminanceThreshold;
float _Gamma;
int _InvertLuminance;
sampler2D _AverageColorTex;
sampler2D _ThresholdTex;
float4 _ThresholdTex_TexelSize;

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);
    fixed4 gammaAdjusted = pow(color, 1 / _Gamma);
    float luminance = (0.2126 * gammaAdjusted.r) +
                    (0.71552 * gammaAdjusted.g) +
                    (0.0722 * gammaAdjusted.b);

    fixed threshold = tex2D(_ThresholdTex, f.uv).r;
    threshold = _InvertLuminance ? 1 - threshold : threshold;

    fixed4 average = tex2D(_AverageColorTex, f.uv);
    return luminance > threshold ? average : 0;
}
