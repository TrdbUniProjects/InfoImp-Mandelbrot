uint getColorPaletteIdx(uint iteration, float a, float b, uint paletteSize) {
    float smoothed = log2(log2(a * a + b * b) / 2.0);
    int idx = ((uint) (native_sqrt(iteration + 10 - smoothed) * 256.0)) % paletteSize;
    return idx;
}

__kernel void mandelbrot(
    __global uint* buffer,
    int xmin,
    int xmax,
    int ymin,
    int ymax,
    uint limit,
    float scale,
    uint paletteSize
) {
    int px = get_global_id(0);
    int py = get_global_id(1);

    float distanceLimit = scale * ((float) ((xmax - xmin) + (ymax - ymin))) / 2.0;

    float x = ((float) xmin) * scale + ((float) px) * scale;
    float y = ((float) ymin) * scale + ((float) py) * scale;

    float a = 0;
    float b = 0;

    uint iteration = 0;
    do {
        float tmpA = a * a - b * b + x;
        b = 2 * a * b + y;
        a = tmpA;
        iteration++;
    } while (distance((float2)(a, b), (float2)(0, 0)) < distanceLimit && iteration < limit);

    buffer[(xmax - xmin) * px + py] = getColorPaletteIdx(iteration, a, b, paletteSize);
}