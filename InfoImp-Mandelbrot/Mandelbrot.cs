using System;
using Eto.Drawing;

namespace InfoImp_Mandelbrot; 

public static class Mandelbrot {
    public static Color[,] GenerateMandelbrot(int limit, Size size, double xMin, double xMax, double yMin, double yMax, double scale) {
        Color[,] result = new Color[size.Width, size.Height];
        
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
                
                result[pixX, pixY] = color;
            }
        }
        
        return result;
    }

    private static Color GetPixelColor(int mandelnr) {
        if (mandelnr < 5) {
            return Colors.Aqua;
        }

        if (mandelnr < 20) {
            return Colors.Orange;
        }

        if (mandelnr < 50) {
            return Colors.Red;
        }
        
        if (mandelnr < 100) {
            return Colors.Green;
        }

        return mandelnr % 2 == 0 ? Colors.Aquamarine : Colors.Azure;
    }
}