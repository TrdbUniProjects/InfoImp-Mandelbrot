using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class CenterYField : InputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "0"
    };
    
    public override (string, TextBox) GetInputField() {
        return ("Midden Y", this.TextBox);
    }
}