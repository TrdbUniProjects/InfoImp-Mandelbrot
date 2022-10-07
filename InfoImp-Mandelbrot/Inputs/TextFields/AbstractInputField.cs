using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public abstract class AbstractInputField {
    protected abstract (string, TextBox) GetInputField();

    public string GetLabel() {
        return this.GetInputField().Item1;
    }

    public TextBox GetInputControl() {
        return this.GetInputField().Item2;
    }
}