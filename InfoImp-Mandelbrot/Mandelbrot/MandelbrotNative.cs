using System.Runtime.InteropServices;
using System.Security;

namespace InfoImp_Mandelbrot.Mandelbrot; 

public static class MandelbrotNative {


    [SuppressUnmanagedCodeSecurity]
    [DllImport("mandelbrot_rayon", EntryPoint = "calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern long FFI_CalculateMandelbrotSet(
        double cx, 
        double cy, 
        int width, 
        int height, 
        int limit,
        double scale, 
        int colorPalette,
        int[] result);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("mandelbrot_ocl", EntryPoint = "ocl_calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern long FFI_OCL_CalculateMandelbrotSet(
        double cx,
        double cy,
        int width,
        int height,
        int limit,
        double scale,
        int colorPalette,
        int[] result);
    
    /// <summary>
    /// Calculate the mandelbrot set
    /// </summary>
    /// <param name="useOcl">Whether to use OpenCL</param>
    /// <param name="cx">Center X</param>
    /// <param name="cy">Center Y</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="limit">Iteration limit</param>
    /// <param name="scale">The scale to use</param>
    /// <param name="colorPalette">The color palette to use</param>
    /// <returns>A tuple with the result and the time it took to calculate in miliseconds</returns>
    public static (int[], long) CalculatemandelbrotSet(
        bool useOcl,
        double cx,
        double cy,
        int width,
        int height,
        int limit,
        double scale,
        int colorPalette
    ) {
        int[] result = new int[width * height];

        long calcTimeMs;
        if(useOcl) {
            calcTimeMs = FFI_OCL_CalculateMandelbrotSet(
                cx,
                cy,
                width,
                height,
                limit,
                scale,
                colorPalette,
                result);
        } else {
            calcTimeMs = FFI_CalculateMandelbrotSet(
                cx,
                cy,
                width,
                height,
                limit,
                scale,
                colorPalette,
                result);   
        }

        return (result, calcTimeMs);
    }
}