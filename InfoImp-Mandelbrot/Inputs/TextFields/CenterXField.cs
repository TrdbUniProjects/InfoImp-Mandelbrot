using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields;

public class CenterXField : AbstractInputField {

    private TextBox TextBox { get; set; } = new TextBox() {
        Text = "0",
    };

    protected override (string, TextBox) GetInputField() {
        return ("Midden X", this.TextBox);
    }
}