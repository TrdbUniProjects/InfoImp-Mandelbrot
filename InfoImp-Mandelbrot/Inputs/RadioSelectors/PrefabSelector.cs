using System.Globalization;
using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public class PrefabSelector : AbstractRadioSelector {
    public PrefabSelector(MainForm mainForm) : base(mainForm) {}

    public override RadioButtonList Radios { get; } = new RadioButtonList() {
        Items = {
            "A",
            "B",
            "C"
        },
        ToolTip = "Select preconfigured parameters"
    };
    
    /// <summary>
    /// Get the prefab selector layout
    /// </summary>
    /// <returns>
    /// The prefab selector
    /// </returns>
    public override StackLayout GetLayout() {
        this.Radios.SelectedKey = "A";
        this.Radios.SelectedIndexChanged += this.OnPrefabListChanged;

        return new StackLayout() {
            Items = {
                "Prefabs",
                this.Radios,
            },
            Padding = new Padding() {
                Bottom = 10,
                Top = 10,
            }
        };    
    }
    
    
    /// <summary>
    /// Called when the prefab list changed
    /// </summary>
    /// <param name="sender">The object which triggered this event</param>
    /// <param name="args">Event arguments</param>
    /// <exception cref="InvalidDataException">If the active index key is not known </exception>
    private void OnPrefabListChanged(object? sender, EventArgs args) {
        double scale, cx, cy;
        int limit = 100;
        int size = 400;
            
        // At this point the prefab list is initialized
        switch (this.Radios.SelectedKey) {
            case "A":
                cx = 0;
                cy = 0;
                scale = 1;
                break;
            case "B":
                cx = -0.7484491448197463;
                cy = -0.04948979034320733;
                scale = 2824.5855390489;
                size = 1000;
                limit = 1000;
                break;
            case "C":
                cx = 0;
                cy = 100;
                scale = 0.005;
                break;
            default:
                throw new InvalidDataException($"Invalid prefab key {this.Radios.SelectedKey}");
        }

        this.MainForm.CenterXField.GetInputControl().Text = cx.ToString(CultureInfo.InvariantCulture);
        this.MainForm.CenterYField.GetInputControl().Text = cy.ToString(CultureInfo.InvariantCulture);
        this.MainForm.ScaleField.GetInputControl().Text = scale.ToString(CultureInfo.InvariantCulture);
        this.MainForm.LimitField.GetInputControl().Text = limit.ToString();
        this.MainForm.SizeField.GetInputControl().Text = size.ToString();

        this.MainForm.MandelView.Cx = cx;
        this.MainForm.MandelView.Cy = cy;
        this.MainForm.MandelView.Scale = scale;
        this.MainForm.MandelView.Limit = limit;
        this.MainForm.MandelView.MandelWidth = this.MainForm.MandelView.MandelHeight = size;
        this.MainForm.MandelView.Invalidate();
    }
}