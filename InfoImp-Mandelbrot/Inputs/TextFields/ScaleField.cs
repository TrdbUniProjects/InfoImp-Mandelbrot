using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class ScaleField : AbstractInputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "1"
    };

    protected override (string, TextBox) GetInputField() {
        return ("Schaal", this.TextBox);
    }
}