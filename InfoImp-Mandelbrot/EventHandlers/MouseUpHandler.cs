namespace InfoImp_Mandelbrot.EventHandlers;

public class MouseUpHandler : IEventHandler {

    private readonly MainForm _mainForm;

    public MouseUpHandler(ref MainForm mainform) {
        this._mainForm = mainform;
    }

    /// <summary>
    /// Called when the user stops pressing their mouse button when inside of the mandelbrot viewer
    /// </summary>
    /// <param name="sender">The object which triggered this event</param>
    /// <param name="args">Arguments to the event</param>
    public void OnEvent(object? sender, EventArgs args) {
        this._mainForm.IsMouseDown = false;
    }
}