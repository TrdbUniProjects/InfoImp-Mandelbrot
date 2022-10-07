using System.Globalization;
using Eto.Drawing;

namespace InfoImp_Mandelbrot.EventHandlers; 

public class GoButtonHandler : IEventHandler {
        
    private readonly MainForm _main;

    public GoButtonHandler(ref MainForm main) {
        this._main = main;
    }

    /// <summary>
    /// This event is called when the 'GO' button is clicked.
    /// This event handler will draw the mandelbrot according to the provided parameters
    /// </summary>
    /// <param name="sender">The object which triggered this event</param>
    /// <param name="args">Arguments to the event</param>
    public void OnEvent(object? sender, EventArgs args) {
        this._main.MandelView.Scale = double.Parse(this._main.ScaleField.GetInputControl().Text, CultureInfo.InvariantCulture);
        this._main.MandelView.Limit = int.Parse(this._main.LimitField.GetInputControl().Text, CultureInfo.InvariantCulture);
        this._main.MandelView.Cx = double.Parse(this._main.CenterXField.GetInputControl().Text, CultureInfo.InvariantCulture);
        this._main.MandelView.Cy = double.Parse(this._main.CenterYField.GetInputControl().Text, CultureInfo.InvariantCulture);
        
        int size = int.Parse(this._main.SizeField.GetInputControl().Text, CultureInfo.InvariantCulture);
        this._main.MandelView.MandelWidth = size;
        this._main.MandelView.MandelHeight = size;
        
        this._main.MandelView.Ready = true;
        this._main.MandelView.Invalidate();
    }
}