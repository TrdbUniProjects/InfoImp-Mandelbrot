using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class CenterYField : AbstractInputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "0"
    };

    protected override (string, TextBox) GetInputField() {
        return ("Midden Y", this.TextBox);
    }
}