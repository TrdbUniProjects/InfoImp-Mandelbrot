using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public class ColorPaletteSelector : AbstractRadioSelector {

    public ColorPaletteSelector(MainForm mainForm) : base(mainForm) {}
    
    public const int ColorPaletteDefault = 0;
    public const int ColorPaletteRedscale = 1;

    public override RadioButtonList Radios { get; } = new RadioButtonList() {
        Items = {
            "Default",
            "Redscale"
        }
    };
    
    /// <summary>
    /// Get the color palette selector layout
    /// </summary>
    /// <returns>
    /// The color palette selector layout
    /// </returns>
    public override StackLayout GetLayout() {
        this.Radios.SelectedKey = "Default";
        this.Radios.SelectedIndexChanged += this.OnColorSelectorChanged;
            
        return new StackLayout() {
            Items = {
                "Color palettes",
                this.Radios,
            },
            Padding = new Padding() {
                Bottom = 10,
                Top = 10,
            }
        };
    }

    private void OnColorSelectorChanged(object? sender, EventArgs args) {
        // colorList is not null at this point
        switch (this.Radios.SelectedKey) {
            case "Default":
                this.MainForm.MandelView.ColorPalette = ColorPaletteDefault;
                break;
            case "Redscale":
                this.MainForm.MandelView.ColorPalette = ColorPaletteRedscale;
                break;
            default:
                throw new InvalidDataException($"Invalid color palette key {this.Radios.SelectedKey}");
        }

        this.MainForm.MandelView.Invalidate();
    }

}