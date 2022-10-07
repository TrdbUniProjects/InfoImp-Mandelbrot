using Eto.Drawing;
using Eto.Forms;
using InfoImp_Mandelbrot.Inputs.RadioSelectors;

namespace InfoImp_Mandelbrot.Mandelbrot; 

public class MandelView : Drawable {

    public Platform BackendPlatform { get; set; } = InfoImp_Mandelbrot.Platform.CSharp;
    public int ColorPalette { get; set; } = ColorPaletteSelector.ColorPaletteDefault;
    public bool Ready { get; set; }
    public double Cx { get; set; }
    public double Cy { get; set; }
    public int MandelWidth { get; set; }
    public int MandelHeight { get; set; }
    public int Limit { get; set; }
    
    public double Scale { get; set; }
    
    protected override void OnPaint(PaintEventArgs args) {
        // The draw call is initiall called when the screen renders
        // if the user hasn't pressed the 'Go' button yet,
        // we'll just draw a black rectangle
        if (!this.Ready) {
            args.Graphics.FillRectangle(Colors.Black, args.ClipRectangle);
            return;
        }

        int[] mandelbrotSet;
        switch (this.BackendPlatform) { 
            case InfoImp_Mandelbrot.Platform.CSharp:
                mandelbrotSet = Mandelbrot.CalculateMandelbrotSet(this.Cx, this.Cy, this.MandelWidth, this.MandelHeight, this.Limit, this.Scale, this.ColorPalette);
                break; 
            case InfoImp_Mandelbrot.Platform.RustOcl:
            case InfoImp_Mandelbrot.Platform.Rust:
                mandelbrotSet = MandelbrotNative.CalculatemandelbrotSet(this.BackendPlatform == InfoImp_Mandelbrot.Platform.RustOcl, this.Cx, this.Cy, this.MandelWidth, this.MandelHeight, this.Limit, this.Scale, this.ColorPalette);
                break;
            default:
                throw new InvalidDataException($"Platform {this.BackendPlatform} is not implemented");
        }
        
        Bitmap bitmap = new Bitmap(this.MandelWidth, this.MandelHeight, PixelFormat.Format24bppRgb);
        BitmapData innerData = bitmap.Lock();
        for (int x = 0; x < this.MandelWidth; x++) {
            for (int y = 0; y < this.MandelHeight; y++) {
                Color c = Color.FromArgb(mandelbrotSet[this.MandelWidth * x + y]);
                innerData.SetPixel(x, y, c);
            }
        }
        
        innerData.Dispose();
            
        args.Graphics.Clear();
        args.Graphics.DrawImage(bitmap, args.ClipRectangle);
        args.Graphics.Flush();
        
        base.OnPaint(args);
    }
}