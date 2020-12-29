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
    }

    return result;
}



fixed4 fp(v2f f) : SV_TARGET {
    fixed4 color = tex2D(_MainTex, f.uv);

    fixed4 lowerBoundContrast = generalSmoothStep(floor(_Contrast), color);
    fixed4 upperBoundContrast = generalSmoothStep(floor(_Contrast) + 1, color);

    return lerp(lowerBoundContrast, upperBoundContrast, frac(_Contrast));
}
