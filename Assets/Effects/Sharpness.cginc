float _Sharpness;

fixed4 fp(v2f f) : SV_TARGET {
    float3x3 kernel = {
        0, -1, 0,
        -1, 5, -1,
        0, -1, 0,
    };

    kernel *= _Sharpness;
    half4 result = 0;

    for (int x = -1; x < 2; ++x) {
        for (int y = -1; y < 2; ++y) {
            fixed2 offset = fixed2(x, y) * _MainTex_TexelSize.xy;
            result += tex2D(_MainTex, f.uv + offset.xy) * kernel[x + 1][y + 1];
        }
    }

    return saturate(result);
}
