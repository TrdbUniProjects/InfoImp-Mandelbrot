using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields; 

public class LimitField : AbstractInputField {
    
    private TextBox TextBox { get; } = new TextBox() {
        Text = "100"
    };

    protected override (string, TextBox) GetInputField() {
        return ("Limiet", this.TextBox);
    }
}