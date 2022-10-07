using System.Runtime.InteropServices;
using System.Security;

namespace InfoImp_Mandelbrot; 

public static class MandelbrotNative {


    [SuppressUnmanagedCodeSecurity]
    [DllImport("mandelbrot_rayon", EntryPoint = "calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern void FFI_CalculateMandelbrotSet(
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
    private static extern void FFI_OCL_CalculateMandelbrotSet(
        double cx,
        double cy,
        int width,
        int height,
        int limit,
        double scale,
        int colorPalette,
        int[] result);
    
    public static int[] CalculatemandelbrotSet(
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
        
        if(useOcl) {
            FFI_OCL_CalculateMandelbrotSet(
                cx,
                cy,
                width,
                height,
                limit,
                scale,
                colorPalette,
                result);
        } else {
            FFI_CalculateMandelbrotSet(
                cx,
                cy,
                width,
                height,
                limit,
                scale,
                colorPalette,
                result);   
        }

        return result;
    }
}