uint getColorPaletteIdx(uint iteration, float a, float b, uint paletteSize) {
    float smoothed = log2(log2(a * a + b * b) / 2.0);
    int idx = ((uint) (native_sqrt(iteration + 10 - smoothed) * 256.0)) % paletteSize;
    return idx;
}

__kernel void mandelbrot(
    __global uint* buffer,
    float cx,
    float cy,
    int width,
    int height,
    uint limit,
    float scale,
    uint paletteSize
) {
    int px = get_global_id(0);
    int py = get_global_id(1);

    float x = (px + (cx / (100 * scale)) - 200) * scale;
    float y = (py + (cy / (100 * scale)) - 200) * scale;

    float a = 0;
    float b = 0;

    uint iteration = 0;
    do {
        float tmpA = a * a - b * b + x;
        b = 2 * a * b + y;
        a = tmpA;
        iteration++;
    } while (distance((float2)(a, b), (float2)(0, 0)) < 2 && iteration < limit);

    buffer[width * px + py] = getColorPaletteIdx(iteration, a, b, paletteSize);
}