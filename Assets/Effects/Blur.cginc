#define TWO_PI 6.28319
#define E 2.71828

float _Blur;
int _Radius;

float gaussian(int x, int y) {
    float sigmaSqu = _Blur * _Blur;
    return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -((x * x) + (y* y)) / (2 * sigmaSqu));
}

fixed4 fp(v2f f) : SV_TARGET {
    half4 result = 0;
    float kernelSum = 0.0f;

    for (int x = -_Radius; x <= _Radius; ++x) {
        for (int y = -_Radius; y <= _Radius; ++y) {
            fixed2 offset = fixed2(x, y) * _MainTex_TexelSize.xy;
            float gauss = gaussian(x, y);
            kernelSum += gauss;
            result += tex2D(_MainTex, f.uv + offset.xy) * gauss;
        }
    }

    result /= kernelSum;
    return saturate(result);
}
