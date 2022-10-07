using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class ScaleField : InputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "1"
    };
    
    public override (string, TextBox) GetInputField() {
        return ("Schaal", this.TextBox);
    }
}