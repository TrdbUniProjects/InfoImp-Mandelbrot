using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class LimitField : InputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "100"
    };
    
    public override (string, TextBox) GetInputField() {
        return ("Limiet", this.TextBox);
    }
}