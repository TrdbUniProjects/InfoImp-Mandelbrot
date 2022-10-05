using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot; 

public class MandelView : Drawable {

    public Platform BackendPlatform { get; set; } = InfoImp_Mandelbrot.Platform.CSharp;
    public int ColorPalette { get; set; } = Mandelbrot.ColorPaletteDefault;
    public bool Ready { get; set; }
    public int XMin { get; set; }
    public int XMax { get; set; }
    public int YMin { get; set; }
    public int YMax { get; set; }
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
                mandelbrotSet = Mandelbrot.CalculateMandelbrotSet(this.XMin, this.XMax, this.YMin, this.YMax, this.Limit, this.Scale, this.ColorPalette);
                break; 
            case InfoImp_Mandelbrot.Platform.Rust:
                mandelbrotSet = MandelbrotNative.CalculatemandelbrotSet(this.XMin, this.XMax, this.YMin, this.YMax, this.Limit, this.Scale, this.ColorPalette);
                break;
            default:
                throw new InvalidDataException($"Platform {this.BackendPlatform} is not implemented");
        }
        
        Bitmap bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
        BitmapData innerData = bitmap.Lock();
            
        for (int x = 0; x < this.Width; x++) {
            for (int y = 0; y < this.Height; y++) {
                Color c = Color.FromArgb(mandelbrotSet[this.Width * x + y]);
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