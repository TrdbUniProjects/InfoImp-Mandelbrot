using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields;

public class WidthField : InputField {

    private TextBox TextBox { get; } = new TextBox() {
        Text = "400"
    };
    
    public override (string, TextBox) GetInputField() {
        return ("Breedte", this.TextBox);
    }
}