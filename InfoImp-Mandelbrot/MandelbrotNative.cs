using System.Runtime.InteropServices;
using System.Security;

namespace InfoImp_Mandelbrot; 

public static class MandelbrotNative {


    [SuppressUnmanagedCodeSecurity]
    [DllImport("mandelbrot_rayon", EntryPoint = "calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern void FFI_CalculateMandelbrotSet(
        int xMin, 
        int xMax, 
        int yMin, 
        int yMax, 
        int limit,
        double scale, 
        int colorPalette,
        int[] result);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("mandelbrot_ocl", EntryPoint = "ocl_calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern void FFI_OCL_CalculateMandelbrotSet(
        int xMin,
        int xMax,
        int yMin,
        int yMax,
        int limit,
        double scale,
        int colorPalette,
        int[] result);
    
    public static int[] CalculatemandelbrotSet(
        bool useOcl,
        int xMin,
        int xMax,
        int yMin,
        int yMax,
        int limit,
        double scale,
        int colorPalette
    ) {
        int width = xMax - xMin;
        int height = yMax - yMin;
        int[] result = new int[width * height];
        
        if(useOcl) {
            FFI_OCL_CalculateMandelbrotSet(
                xMin,
                xMax,
                yMin,
                yMax,
                limit,
                scale,
                colorPalette,
                result);
        } else {
            FFI_CalculateMandelbrotSet(
                xMin,
                xMax,
                yMin,
                yMax,
                limit,
                scale,
                colorPalette,
                result);   
        }

        return result;
    }
}