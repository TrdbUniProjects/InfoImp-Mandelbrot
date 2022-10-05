using Eto.Drawing;

namespace InfoImp_Mandelbrot; 

public static class Mandelbrot {

    private static readonly Color[] Colors = new Color[] {
        Color.FromArgb(66, 30, 15),
        Color.FromArgb(25, 7, 26),
        Color.FromArgb(9, 1, 47),
        Color.FromArgb(4, 4, 73),
        Color.FromArgb(0, 7, 100),
        Color.FromArgb(12, 44, 138),
        Color.FromArgb(24, 82, 177),
        Color.FromArgb(57, 125, 209),
        Color.FromArgb(134, 181, 229),
        Color.FromArgb(211, 236, 248),
        Color.FromArgb(241, 233, 191),
        Color.FromArgb(248, 201, 95),
        Color.FromArgb(255, 170, 0),
        Color.FromArgb(204, 128, 0),
        Color.FromArgb(153, 87, 0),
        Color.FromArgb(106, 52, 3)
    };
    
    private static double Distance(double xa, double ya, double xb, double yb) {
        return Math.Sqrt((xa * xa + ya * ya) - (xb * xb + yb * yb));
    }

    /// <summary>
    /// Calculate the mandelbrot set for the provided paraemeters
    /// </summary>
    /// <param name="xMin">The top left X coordinate from where to start calculating</param>
    /// <param name="xMax">The bottom right X coordinate to calculate to</param>
    /// <param name="yMin">The top left Y coordinate from where to start calculating</param>
    /// <param name="yMax">The bottom right Y coordinate to calculate to</param>
    /// <param name="limit">The iteration limit in the mandelbrot formula</param>
    /// <param name="scale">The scale of the image (resolution)</param>
    /// <returns>
    /// The ARGB color value for every pixel in a 2D array
    /// </returns>
    public static int[,] CalculateMandelbrotSet(int xMin, int xMax, int yMin, int yMax, int limit, double scale) {
        int width = xMax - xMin;
        int height = yMax - yMin;
        
        int[,] result = new int[width, height];

        for (int px = 0; px < width; px++) {
            for (int py = 0; py < height; py++) {
                double x = xMin * scale + scale * px;
                double y = yMin * scale + scale * py;

                double a = 0;
                double b = 0;

                int iteration = 0;
                do {
                    double tmpA = a = a * a - b * b + x;
                    b = 2 * a * b + y;
                    a = tmpA;
                    iteration++;
                } while (Distance(a, b, 0, 0) < (scale * (width + height) / 2) && iteration < limit);

                result[px, py] = GetPixelColor(iteration, a, b).ToArgb();
            }
        }

        return result;
    }
    

    private static Color GetPixelColor(int iteration, double a, double b) {
        double smoothed = Math.Log2(Math.Log2(a * a + b * b) / 2);
        int idx = (int)(Math.Sqrt(iteration + 10 - smoothed) * 256) % Colors.Length;
        return Colors[idx];
    }
}