using Eto.Forms;

namespace InfoImp_Mandelbrot.EventHandlers; 

public class MouseDownHandler : IEventHandler {
    /// <summary>
    /// The scale step factor applied when zooming in or zooming out
    /// </summary>
    private const double ScaleStepFactor = 0.1;
    /// <summary>
    /// The minimum duration in miliseconds for which the mouse button has to be pressed,
    /// after which the mandelbrot viewer will zoom in every n miliseconds.
    /// Where n is the value of this variable
    /// </summary>
    private const int MouseDownConstantZoomMinimumDurationMs = 1000;
    
    private readonly MainForm _main;
    public MouseDownHandler(ref MainForm main) {
        this._main = main;
    }
    
    /// <summary>
    /// Called when the user presses a mouse button within the mandelbrot viewer.
    /// This function will update the X and Y coordinate of the mandelbrot to the user's mouse position,
    /// and adjust the scale of the mandelbrot.
    /// </summary>
    /// <param name="sender">The object that triggered this event</param>
    /// <param name="eargs">The arguments to this event</param>
    public void OnEvent(object? sender, EventArgs eargs) {
        MouseEventArgs args = (MouseEventArgs) eargs;
        
        // We don't use the mandel's size here, as the user-visible image is only 400x400+
        // Original transformation:
        //double normalMouseX = args.Location.X * 4 / 400f;
        // Optimised:
        double normalMouseX = args.Location.X * 0.01f;
        double normalMouseY = args.Location.Y * 0.01f;

        double centeredMouseX = normalMouseX - 2.0;
        double centeredMouseY = normalMouseY - 2.0;
            
        this.ApplyZoom(args.Buttons);
        double scale = this._main.MandelView.Scale;
            
        this._main.MandelView.Cx += centeredMouseX / scale;
        this._main.MandelView.Cy += centeredMouseY / scale;
            
        DateTimeOffset initialMouseDown = DateTimeOffset.Now;
        this._main.IsMouseDown = true;
        Thread t = new Thread(() => {
            while (this._main.IsMouseDown) {
                long deltaMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - initialMouseDown.ToUnixTimeMilliseconds();
                if (MouseDownConstantZoomMinimumDurationMs > deltaMillis) {
                    continue;
                }
                
                Application.Instance.Invoke(() => {
                    this.ApplyZoom(args.Buttons);
                    this._main.MandelView.Invalidate();
                });
                initialMouseDown = DateTimeOffset.Now;
            }
        });
        t.Start();

        this._main.CenterXField.GetInputControl().Text = this._main.MandelView.Cx.ToString(Thread.CurrentThread.CurrentCulture);
        this._main.CenterYField.GetInputControl().Text = this._main.MandelView.Cy.ToString(Thread.CurrentThread.CurrentCulture);
            
        this._main.MandelView.Invalidate();
    }
    
    /// <summary>
    /// Apply zoom to the mandelbrot viewer based on the mouse button pressed.
    /// Note that this does not rerender the mandelbrot
    /// </summary>
    /// <param name="buttons">The mouse buttons pressed</param>
    private void ApplyZoom(MouseButtons buttons) {
        // Left = zoom in
        // Right = zoom out
        switch (buttons) {
            case MouseButtons.Primary:
                this._main.MandelView.Scale += this._main.MandelView.Scale * ScaleStepFactor;
                break;
            case MouseButtons.Alternate:
                this._main.MandelView.Scale -= this._main.MandelView.Scale * ScaleStepFactor;
                break;
        }
            
        this._main.ScaleField.GetInputControl().Text = this._main.MandelView.Scale.ToString(Thread.CurrentThread.CurrentCulture);
    }

}