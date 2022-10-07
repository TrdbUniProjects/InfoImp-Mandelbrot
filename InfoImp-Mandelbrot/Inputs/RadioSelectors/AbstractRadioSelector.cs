using Eto.Forms;

namespace InfoImp_Mandelbrot.Inputs.RadioSelectors; 

public abstract class AbstractRadioSelector {

    protected readonly MainForm MainForm;

    protected AbstractRadioSelector(MainForm mainForm) {
        this.MainForm = mainForm;
    }
    
    public abstract RadioButtonList Radios { get; }

    public abstract StackLayout GetLayout();
}