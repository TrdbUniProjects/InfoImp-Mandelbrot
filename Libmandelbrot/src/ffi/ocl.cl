#if defined(cl_khr_fp64)  // Khronos extension available?

#pragma OPENCL EXTENSION cl_khr_fp64 : enable
#define DOUBLE_SUPPORT_AVAILABLE

#elif defined(cl_amd_fp64)  // AMD extension available?

#pragma OPENCL EXTENSION cl_amd_fp64 : enable
#define DOUBLE_SUPPORT_AVAILABLE

#endif

#if defined(DOUBLE_SUPPORT_AVAILABLE)

// double
typedef double real_t;

#else

// float
typedef float real_t;

#endif // DOUBLE_SUPPORT_AVAILABLE


uint getColorPaletteIdx(uint iteration, real_t a, real_t b, uint paletteSize) {
    real_t smoothed = log2(log2(a * a + b * b) / 2.0);
    int idx = ((uint) (native_sqrt(iteration + 10 - smoothed) * 256.0)) % paletteSize;
    return idx;
}

__kernel void mandelbrot(
    __global uint* buffer,
    real_t cx,
    real_t cy,
    int width,
    int height,
    uint limit,
    real_t scalar,
    uint paletteSize
) {
    int px = get_global_id(0);
    int py = get_global_id(1);

    real_t x = cx + ((real_t) px / width * 4.0 - 2.0) * scalar;
    real_t y = cy + ((real_t) py / height * 4.0 - 2.0) * scalar;

    real_t a = 0;
    real_t b = 0;

    real_t aSquared;
    real_t bSquared;

    uint iteration = 0;
    do {
        aSquared = a * a;
        bSquared = b * b;

        real_t tmpA = aSquared - bSquared + x;
        b = 2 * a * b + y;
        a = tmpA;
        iteration++;
    } while (aSquared + bSquared < 4 && iteration < limit);

    buffer[width * px + py] = getColorPaletteIdx(iteration, a, b, paletteSize);
}