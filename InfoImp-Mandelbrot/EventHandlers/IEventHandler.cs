namespace InfoImp_Mandelbrot.EventHandlers; 

public interface IEventHandler {
    void OnEvent(object? sender, EventArgs args);
}