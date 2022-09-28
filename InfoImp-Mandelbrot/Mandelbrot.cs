using System;
using Eto.Drawing;

namespace InfoImp_Mandelbrot; 

public static class Mandelbrot {

    private static int AdjustColorScale(float i) {
        return (int)Math.Floor(255 * i);
    }
    private static int ColorToRgb(Color c) {
        int rgb = AdjustColorScale(c.R);
        rgb = (rgb << 8) + AdjustColorScale(c.G);
        rgb = (rgb << 8) + AdjustColorScale(c.B);

        return rgb;
    }


    private static int Distance(int xa, int ya, int xb, int yb) {
        return (int) Math.Sqrt((xa * xa + ya * ya) - (xb * xb + yb * yb));
    }
    public static int[,] GenerateMandelbrotV2(int cx, int cy, double scale, int limit, int size) {
        int[,] result = new int[size, size];
        for (int px = 0; px < size; px++) {
            for (int py = 0; py < size; py++) {
                int iteration = 0;

                int a = 0;
                int b = 0;

                do {
                    a = (a * a - b * b + px);
                    b = (2 * a * b + py);

                    iteration++;
                } while (iteration < limit && Distance(cx, cy, a, b) < scale);

                result[px, py] = GetPixelColor(iteration).ToArgb();
            }
        }

        return result;
    }

    public static int[,] GenerateMandelbrot(int limit, Size size, double xMin, double xMax, double yMin, double yMax, double scale) {
        int[,] result = new int[size.Width, size.Height];
        for (int pixX = 0; pixX < size.Width; pixX++) {
            for (int pixY = 0; pixY < size.Height; pixY++) {
                double x = ((xMax - xMin) * pixX) / (size.Width - 1) + xMin;
                double y = ((yMax - yMin) * pixY) / (size.Height - 1) + yMin;

                int iterations = 0;
                double a = x;
                double b = y;
                do {
                    a = a * a - b * b + x;
                    b = 2 * a * b + y;
                    iterations++;
                } while (iterations < limit && a * a + b * b < scale);
 
                Color color;
                if (iterations > limit) {
                    color = Colors.Black;
                } else {
                    color = Mandelbrot.GetPixelColor(iterations);
                    
                }
                
                result[pixX, pixY] = color.ToArgb();
            }
        }
        
        return result;
    }

    private static Color GetPixelColor(int mandelnr) {
        return mandelnr % 2 == 0 ? Colors.Aquamarine : Colors.Azure;
    }
}