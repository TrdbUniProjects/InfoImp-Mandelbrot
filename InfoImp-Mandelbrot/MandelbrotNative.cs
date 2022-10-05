using System.Runtime.InteropServices;

namespace InfoImp_Mandelbrot; 

public static class MandelbrotNative {
    
    [DllImport("libmandelbrot", EntryPoint = "calculate_mandelbrot_set", CallingConvention = CallingConvention.Winapi)]
    private static extern void FFI_CalculateMandelbrotSet(
        int xMin, 
        int xMax, 
        int yMin, 
        int yMax, 
        int limit,
        double scale, 
        int colorPalette,
        int[] result);

    public static int[] CalculatemandelbrotSet(
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
        
        FFI_CalculateMandelbrotSet(
            xMin,
            xMax,
            yMin,
            yMax,
            limit,
            scale,
            colorPalette,
            result);
        
        return result;
    }
}