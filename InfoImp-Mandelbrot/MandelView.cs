using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot; 

public class MandelView : Drawable {
    
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

        int[,] mandelbrotSet = Mandelbrot.CalculateMandelbrotSet(this.XMin, this.XMax, this.YMin, this.YMax, this.Limit, this.Scale);
        Bitmap bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
        BitmapData innerData = bitmap.Lock();
            
        for (int x = 0; x < this.Width; x++) {
            for (int y = 0; y < this.Height; y++) {
                Color c = Color.FromArgb(mandelbrotSet[x, y]);
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