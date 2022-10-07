using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public class PrefabSelector : RadioSelector {
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
        int cx, cy;
        double scale;
            
        // At this point the prefab list is initialized
        switch (this.Radios.SelectedKey) {
            case "A":
                cx = 0;
                cy = 0;
                scale = 1;
                break;
            case "B":
                cx = 100;
                cy = 100;
                scale = 1000;
                break;
            case "C":
                cx = 0;
                cy = 100;
                scale = 0.005;
                break;
            default:
                throw new InvalidDataException($"Invalid prefab key {this.Radios.SelectedKey}");
        }

        this.MainForm.CenterXField.GetInputControl().Text = cx.ToString();
        this.MainForm.CenterYField.GetInputControl().Text = cy.ToString();
        this.MainForm.ScaleField.GetInputControl().Text = scale.ToString(Thread.CurrentThread.CurrentCulture);

        this.MainForm.MandelView.Cx = cx;
        this.MainForm.MandelView.Cy = cy;
        this.MainForm.MandelView.Scale = scale;
        this.MainForm.MandelView.Invalidate();
    }
}