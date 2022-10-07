using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.TextFields;

public class CenterXField : InputField {

    private TextBox TextBox { get; set; } = new TextBox() {
        Text = "0",
    };
    
    public override (string, TextBox) GetInputField() {
        return ("Midden X", this.TextBox);
    }
}