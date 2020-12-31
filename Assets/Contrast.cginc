float _Contrast;

//Generalized Smoothstep formula from Smoothstep wikipedia page
float pascalTriangle(float a, float b) {
    float result = 1;
    for (int i = 0; i < b; ++i) {
        result *= (a - i) / (i + 1);
    }

    return result;
}

fixed4 generalSmoothStep(int n, fixed4 color) {
    color = saturate(color);
    fixed4 result = 0;

    for (int i = 0; i <= n; ++i) {
        result += pascalTriangle(-n - 1, i) *
                  pascalTriangle(2 * n + 1, n - i) *
                  pow(color, n + i + 1);

        if (dot(result, 1) > 1) return result;
    }

    return result;
}

fixed4 inverseSmoothStep(int n, fixed4 color) {
    fixed4 result = saturate(color);
    result.r = 0.5 - sin(asin(1.0 - 2.0 * result.r) / (3.0 + n));
    result.g = 0.5 - sin(asin(1.0 - 2.0 * result.g) / (3.0 + n));
    result.b = 0.5 - sin(asin(1.0 - 2.0 * result.b) / (3.0 + n));

    return saturate(result);
}

fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);

    if (_Contrast > 0) {
        fixed4 lowerBoundContrast = generalSmoothStep(floor(_Contrast), color);
        fixed4 upperBoundContrast = generalSmoothStep(floor(_Contrast) + 1, color);

        color = lerp(lowerBoundContrast, upperBoundContrast, frac(_Contrast));
    } else if (_Contrast < 0) {
        color = inverseSmoothStep(-(_Contrast), color);
    }

    return color;
}