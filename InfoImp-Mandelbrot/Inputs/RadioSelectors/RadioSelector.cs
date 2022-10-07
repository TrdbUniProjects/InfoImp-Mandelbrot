using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public abstract class RadioSelector {

    protected readonly MainForm MainForm;

    public RadioSelector(MainForm mainForm) {
        this.MainForm = mainForm;
    }
    
    public abstract RadioButtonList Radios { get; }

    public abstract StackLayout GetLayout();
}