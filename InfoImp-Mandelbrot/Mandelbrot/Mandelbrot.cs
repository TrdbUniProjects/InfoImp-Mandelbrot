using Eto.Drawing;
using InfoImp_Mandelbrot.Inputs.RadioSelectors;

namespace InfoImp_Mandelbrot.Mandelbrot; 

public static class Mandelbrot {

    /// <summary>
    /// Precomputed default color shades
    /// </summary>
    private static readonly Color[] DefaultColors = new Color[] {
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
    
    /// <summary>
    /// Precomputed red color shades
    /// </summary>
    private static readonly Color[] RedColors = new Color[] {
        Color.FromArgb(10, 0, 0),
        Color.FromArgb(20, 0, 0),
        Color.FromArgb(30, 0, 0),
        Color.FromArgb(40, 0, 0),
        Color.FromArgb(50, 0, 0),
        Color.FromArgb(60, 0, 0),
        Color.FromArgb(70, 0, 0),
        Color.FromArgb(80, 0, 0),
        Color.FromArgb(90, 0, 0),
        Color.FromArgb(100, 0, 0),
        Color.FromArgb(110, 0, 0),
        Color.FromArgb(120, 0, 0),
        Color.FromArgb(130, 0, 0),
        Color.FromArgb(140, 0, 0),
        Color.FromArgb(150, 0, 0),
        Color.FromArgb(160, 0, 0),
        Color.FromArgb(170, 0, 0),
        Color.FromArgb(180, 0, 0),
        Color.FromArgb(190, 0, 0),
        Color.FromArgb(200, 0, 0),
        Color.FromArgb(210, 0, 0),
        Color.FromArgb(220, 0, 0),
        Color.FromArgb(230, 0, 0),
        Color.FromArgb(240, 0, 0),
        Color.FromArgb(250, 0, 0),
    };
    
    private static double DistanceSquared(double xa, double ya, double xb, double yb) {
        return (xa * xa + ya * ya) - (xb * xb + yb * yb);
    }

    /// <summary>
    /// Calculate the mandelbrot set for the provided paraemeters
    /// </summary>
    /// <param name="cy">The center X coordinate</param>
    /// <param name="cx">The center Y coordinate</param>
    /// <param name="width">The width of the viewport</param>
    /// <param name="height">The height of the viewport</param>
    /// <param name="limit">The iteration limit in the mandelbrot formula</param>
    /// <param name="scale">The scale of the image (resolution)</param>
    /// <param name="colorPalette"> The color palette to use</param>
    /// <returns>
    /// The ARGB color value for every pixel in an array stored in column order
    /// </returns>
    public static int[] CalculateMandelbrotSet(double cx, double cy, int width, int height, int limit, double scale, int colorPalette) {

        int[] result = new int[width * height];

        for (int px = 0; px < width; px++) {
            for (int py = 0; py < height; py++) {
                double npx = px * 4f / width;
                double npy = py * 4f / height;
                
                double x = (npx + cx - 2f) * scale;
                double y = (npy + cy - 2f) * scale;
                
                double a = 0;
                double b = 0;

                int iteration = 0;
                do {
                    double tmpA = a * a - b * b + x;
                    b = 2 * a * b + y;
                    a = tmpA;
                    iteration++;
                } while (DistanceSquared(a, b, 0, 0) < 4 && iteration < limit);

                result[width * px + py] = GetPixelColor(iteration, a, b, colorPalette).ToArgb();
            }
        }
        
        return result;
    }

    private static Color GetPixelColorWithColorSet(int iteration, double a, double b, Color[] colorSet) {
        double smoothed = Math.Log2(Math.Log2(a * a + b * b) / 2);
        if (double.IsNaN(smoothed)) {
            return Colors.Black;
        }
        int idx = (int) (Math.Sqrt(iteration + 10 - smoothed) * 256f) % colorSet.Length;
        return colorSet[idx];
    }

    private static Color GetPixelColor(int iteration, double a, double b, int colorPalette) {
        switch (colorPalette) {
            case ColorPaletteSelector.ColorPaletteDefault:
                return GetPixelColorWithColorSet(iteration, a, b, DefaultColors);
            case ColorPaletteSelector.ColorPaletteRedscale:
                return GetPixelColorWithColorSet(iteration, a, b, RedColors);
            default:
                throw new InvalidDataException($"Invalid color palette {colorPalette}");
        }
    }
}