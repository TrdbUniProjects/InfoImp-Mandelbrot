using System.Diagnostics;
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

        double zoomScalar = 1f / scale;

        Parallel.For(0, height, (int pixelSpaceX) => {
            // Translate X from pixel space to image space
            // We dont need to optimise the 2f - -2f etc, the compiler does this for us
            double imageSpaceX = (double)pixelSpaceX / width * (2f - -2f) + -2f;
            // Apply the center X and zoom scalar
            double x = cx + imageSpaceX * zoomScalar;
            
            for (int pixelSpaceY = 0; pixelSpaceY < height; pixelSpaceY++) {
                // Translate Y from pixel space to image space
                double imageSpaceY = (double)pixelSpaceY / height * (2f - -2f) + -2f;
                // Apply the center y and zoom scalar
                double y = cy + imageSpaceY * zoomScalar;

                double a = 0;
                double b = 0;

                double aSquared;
                double bSquared;

                int iteration = 0;
                do {
                    // Square these seperately to
                    // avoid multiplying them more than needed
                    aSquared = a * a;
                    bSquared = b * b;

                    double tmpA = aSquared - bSquared + x;
                    b = 2 * a * b + y;
                    a = tmpA;
                    iteration++;

                    // Pythagoras tells us that the distance between A and B is equal to
                    // distance = sqrt(xa^2 + ya^2 - (xb^2 + yb^2))
                    // However, since we know the distance (2), we can square that (4) and elide the sqrt
                    // Furthermore, B is 0, 0^2 is still 0, 0 + 0 is zero, N - 0 = N.
                    // Thus we can elide that half.
                    // We've already multiplied a and b before, use that
                } while (aSquared + bSquared < 4 && iteration < limit);

                result[width * pixelSpaceX + pixelSpaceY] = GetPixelColor(iteration, a, b, colorPalette).ToArgb();
            }
        });

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