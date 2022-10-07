using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public abstract class InputField {
    public abstract (string, TextBox) GetInputField();

    public string GetLabel() {
        return this.GetInputField().Item1;
    }

    public TextBox GetInputControl() {
        return this.GetInputField().Item2;
    }
}