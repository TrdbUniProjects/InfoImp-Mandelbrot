using Eto.Drawing;
using Eto.Forms;
using InfoImp_Mandelbrot.EventHandlers;
using InfoImp_Mandelbrot.Inputs.RadioSelectors;
using InfoImp_Mandelbrot.Inputs.TextFields;
using InfoImp_Mandelbrot.Mandelbrot;

namespace InfoImp_Mandelbrot;

public class MainForm : Form {
    public readonly MandelView MandelView;

    public readonly CenterXField CenterXField;
    public readonly CenterYField CenterYField;
    public readonly LimitField LimitField;
    public readonly ScaleField ScaleField;
    public readonly SizeField SizeField;
    private readonly Label _calcTimeLabel = new Label { Text = "0 ms" };
    
    public bool IsMouseDown;

    public MainForm() {
        this.Title = "INFOIMPL Mandelbrot";
        base.Size = new Size(800, 800);
        base.Menu = InfoImp_Mandelbrot.Menu.GetMenuBar(this);
        
        this.CenterXField = new CenterXField();
        this.CenterYField = new CenterYField();
        this.LimitField = new LimitField();
        this.ScaleField = new ScaleField();
        this.SizeField = new SizeField();
        
        // Configure the mandelbrot viewer
        int size = int.Parse(this.SizeField.GetInputControl().Text);
        this.MandelView = new MandelView(this) { Size = new Size(size, size) };
        MainForm self = this;

        this.MandelView.MouseDown += new MouseDownHandler(ref self).OnEvent;
        this.MandelView.MouseUp += new MouseUpHandler(ref self).OnEvent;

        Button goBtn = new Button {
            Width = 400,
            Text = "Go",
        };
        goBtn.Click += new GoButtonHandler(ref self).OnEvent;

        this.Content = new StackLayout() {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Items = {
                // Parameter inputs
                new TableLayout {
                    Spacing = new Size(10, 10),
                    Rows = {
                        new TableRow() {
                            Cells = {
                                this.CenterXField.GetLabel(),
                                this.CenterXField.GetInputControl(),
                                this.LimitField.GetLabel(),
                                this.LimitField.GetInputControl(),
                            }
                        },
                        new TableRow() {
                            Cells = {
                                this.CenterYField.GetLabel(),
                                this.CenterYField.GetInputControl(),
                                this.ScaleField.GetLabel(),
                                this.ScaleField.GetInputControl(),
                            }
                        },
                        new TableRow() {
                            Cells = {
                                this.SizeField.GetLabel(),
                                this.SizeField.GetInputControl(),
                            }
                        }
                    }
                },
                new TableLayout() {
                    Spacing = new Size(20, 0),
                    Rows = {
                        new TableRow() {
                            Cells = {
                                new PrefabSelector(this).GetLayout(),
                                new ColorPaletteSelector(this).GetLayout(),
                                new ComputePlatformSelector(this).GetLayout(),
                            }
                        },
                    }
                },
                new StackLayout() {
                    Items = {
                        goBtn
                    },
                    Padding = new Padding() {
                        Bottom = 10,
                    }
                },
                // mandelbrot viewer
                this.MandelView,
                this._calcTimeLabel,
            }
        };

        // Add padding to the window itself
        this.Padding = new Padding(20);
    }

    public void SetCalculationTime(long ns) {
        if (ns > 1e+9 ) {
            this._calcTimeLabel.Text = $"{ns / 1e+9} seconds";
        } else if (ns > 1000000) {
            this._calcTimeLabel.Text = $"{ns / 1000000} miliseconds";
        }
        else {
            this._calcTimeLabel.Text = $"{ns} nanoseconds";
        }
    }
}