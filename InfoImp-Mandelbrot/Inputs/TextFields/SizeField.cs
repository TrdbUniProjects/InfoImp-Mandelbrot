using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields;

public class SizeField : AbstractInputField {

    private TextBox TextBox { get; } = new TextBox() {
        Text = "400"
    };

    protected override (string, TextBox) GetInputField() {
        return ("Formaat", this.TextBox);
    }
}