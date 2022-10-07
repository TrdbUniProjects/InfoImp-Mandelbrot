using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public class ComputePlatformSelector : AbstractRadioSelector {
    public ComputePlatformSelector(MainForm mainForm) : base(mainForm) {}

    public override RadioButtonList Radios { get; } = new RadioButtonList() {
        ToolTip = "Select the backend to use for calculations. Which options are availasble depends on the platform, If you are using the GPU option, it is your responsibility to ensure libOpenCL is available!"
    };
    
    /// <summary>
    /// Get the backend selector layout.
    /// The options available in the selector depends on the OS and architecutre
    /// </summary>
    /// <returns>
    /// The backend selector layout
    /// </returns>
    public override StackLayout GetLayout() {
        ListItemCollection availableOptions = new ListItemCollection() {
            "C#"
        };

        Platform[] supportedPlatforms = PlatformExtension.GetAvailablePlatforms();
        if (supportedPlatforms.Contains(Platform.Rust)) {
            availableOptions.Add("Rust");
        }

        if (supportedPlatforms.Contains(Platform.RustOcl)) {
            availableOptions.Add("GPU");
        }

        foreach (IListItem item in availableOptions) {
            this.Radios.Items.Add(item);
        }

        this.Radios.SelectedKey = "C#";
        this.Radios.SelectedIndexChanged += this.OnBackendSelectorChanged;

        return new StackLayout() {
            Items = {
                "Backend",
                this.Radios,
            },
            Padding = new Padding() {
                Bottom = 10,
                Top = 10,
            }
        };
    }
    
    private void OnBackendSelectorChanged(object? sender, EventArgs args) {
        Platform p;
        switch (this.Radios.SelectedKey) {
            case "C#":
                p = Platform.CSharp;
                break;
            case "Rust":
                p = Platform.Rust;
                break;
            case "GPU":
                p = Platform.RustOcl;
                break;
            default:
                throw new InvalidDataException($"Unknown platform key {this.Radios.SelectedKey}");
        }

        this.MainForm.MandelView.BackendPlatform = p;
        this.MainForm.MandelView.Invalidate();
    }
}