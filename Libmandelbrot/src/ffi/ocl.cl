uint getColorPaletteIdx(uint iteration, float a, float b, uint paletteSize) {
    float smoothed = log2(log2(a * a + b * b) / 2.0);
    int idx = ((uint) (native_sqrt(iteration + 10 - smoothed) * 256.0)) % paletteSize;
    return idx;
}

float distanceSquared(float xa, float ya, float xb, yb) {
    return (xa * xa + ya * ya) - (xb * xb + yb * yb);
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

    float npx = px * 4.0 / width;
    float npy = py * 4.0 / height;

    float x = (npx + cx - 2.0) * scale;
    float y = (npy + cy - 2.0) * scale;

    float a = 0;
    float b = 0;

    uint iteration = 0;
    do {
        float tmpA = a * a - b * b + x;
        b = 2 * a * b + y;
        a = tmpA;
        iteration++;
    } while (distanceSquared(a, b, 0, 0) < 4 && iteration < limit);

    buffer[width * px + py] = getColorPaletteIdx(iteration, a, b, paletteSize);
}