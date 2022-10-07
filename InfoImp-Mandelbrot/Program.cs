using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot {
    public class MainForm : Form {
        /// <summary>
        /// The scale step factor applied when zooming in or zooming out
        /// </summary>
        private const double ScaleStepFactor = 0.1;
        /// <summary>
        /// The minimum duration in miliseconds for which the mouse button has to be pressed,
        /// after which the mandelbrot viewer will zoom in every n miliseconds.
        /// Where n is the value of this variable
        /// </summary>
        private const int MouseDownConstantZoomMinimumDurationMs = 1000;
        
        private readonly MandelView _mandelView;
        private readonly TextBox _middleXField, _middleYField, _scaleField, _limitField, _widthField, _heightField;

        private bool _isMouseDown;
        private RadioButtonList? _prefabList;
        private RadioButtonList? _colorList;
        private RadioButtonList? _backendList;
        
        public MainForm() {
            this.Title = "INFOIMPL Mandelbrot";
            base.Size = new Size(800, 800);
            KeyValuePair<TableCell[], TextBox> middleXField = BuildLabelledInputRow("Midden X");
            KeyValuePair<TableCell[], TextBox> middleYField = BuildLabelledInputRow("Midden Y");
            KeyValuePair<TableCell[], TextBox> scaleField = BuildLabelledInputRow("Schaal");
            KeyValuePair<TableCell[], TextBox> maxCountField = BuildLabelledInputRow("Max Aantal");
            KeyValuePair<TableCell[], TextBox> widthField = BuildLabelledInputRow("Breedte");
            KeyValuePair<TableCell[], TextBox> heightField = BuildLabelledInputRow("Hoogte");

            // Configure default values
            middleXField.Value.Text = "0";
            middleYField.Value.Text = "0";
            scaleField.Value.Text = "1";
            maxCountField.Value.Text = "100";
            widthField.Value.Text = "400";
            heightField.Value.Text = "400";

            this._middleXField = middleXField.Value;
            this._middleYField = middleYField.Value;
            this._scaleField = scaleField.Value;
            this._limitField = maxCountField.Value;
            this._widthField = widthField.Value;
            this._heightField = heightField.Value;

            // Configure the mandelbrot viewer
            this._mandelView = new MandelView() { Size = new Size(int.Parse(this._widthField.Text), int.Parse(this._heightField.Text)) };
            this._mandelView.MouseDown += this.OnMandelViewMouseDown;
            this._mandelView.MouseUp += this.OnMandelViewMouseUp;

            this.Content = new StackLayout() {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = {
                    // Parameter inputs
                    new TableLayout {
                        Spacing = new Size(10, 10),
                        Rows = {
                            new TableRow() {
                                Cells = {
                                    middleXField.Key[0],
                                    middleXField.Key[1],
                                    middleYField.Key[0],
                                    middleYField.Key[1],
                                },
                            },
                            new TableRow() {
                                Cells = {
                                    scaleField.Key[0],
                                    scaleField.Key[1],
                                    maxCountField.Key[0],
                                    maxCountField.Key[1],
                                }
                            },
                            new TableRow() {
                                Cells = {
                                    widthField.Key[0],
                                    widthField.Key[1],
                                    heightField.Key[0],
                                    heightField.Key[1],
                                }
                            }
                        }
                    },
                    new TableLayout() {
                        Spacing = new Size(20, 0),
                        Rows = {
                            new TableRow() {
                                Cells = {
                                    this.GetPrefabSelectorLayout(),
                                    this.GetColorSelector(),
                                    this.GetBackendSelectorLayout(),

                                }
                            },
                        }
                    },
                    this.GetGoButton(),
                    // mandelbrot viewer
                    this._mandelView
                }
            };

            // Add padding to the window itself
            this.Padding = new Padding(20);
        }

        /// <summary>
        /// Get the backend selector layout.
        /// The options available in the selector depends on the OS and architecutre
        /// </summary>
        /// <returns>
        /// The backend selector layout
        /// </returns>
        private StackLayout GetBackendSelectorLayout() {
            ListItemCollection availableOptions = new ListItemCollection() {
                "C#"
            };

            bool osSupported;
            switch (Environment.OSVersion.Platform) {
                case PlatformID.Win32NT:
                case PlatformID.Unix:
                    osSupported = true;
                    break;
                default:
                    Console.WriteLine($"Native backend not supported on {Environment.OSVersion.Platform}");
                    osSupported = false;
                    break;
            }

            bool archSupported;
            switch (RuntimeInformation.OSArchitecture) {
                case Architecture.X64:
                    archSupported = true;
                    break;
                default:
                    Console.WriteLine($"Native backend not supported on architecture {RuntimeInformation.OSArchitecture}");
                    archSupported = false;
                    break;
            }

            if (osSupported && archSupported) {
                availableOptions.Add("Rust");
                availableOptions.Add("GPU");
            }

            this._backendList = new RadioButtonList() {
                ToolTip = "Select the backend to use for calculations. Which options are availasble depends on the platform, If you are using the GPU option, it is your responsibility to ensure libOpenCL is available!"
            };

            foreach (IListItem item in availableOptions) {
                this._backendList.Items.Add(item);
            }

            this._backendList.SelectedKey = "C#";
            this._backendList.SelectedIndexChanged += this.OnBackendSelectorChanged;

            return new StackLayout() {
                Items = {
                    "Backend",
                    this._backendList,
                },
                Padding = new Padding() {
                    Bottom = 10,
                    Top = 10,
                }
            };
        }
        
        private void OnBackendSelectorChanged(object? sender, EventArgs args) {
            Platform p;
            switch (this._backendList!.SelectedKey) {
                case "C#":
                    p = InfoImp_Mandelbrot.Platform.CSharp;
                    break;
                case "Rust":
                    p = InfoImp_Mandelbrot.Platform.Rust;
                    break;
                case "GPU":
                    p = InfoImp_Mandelbrot.Platform.RustOcl;
                    break;
                default:
                    throw new InvalidDataException($"Unknown platform key {this._backendList.SelectedKey}");
            }

            this._mandelView.BackendPlatform = p;
            this._mandelView.Invalidate();
        }
        
        /// <summary>
        /// Get the color palette selector layout
        /// </summary>
        /// <returns>
        /// The color palette selector layout
        /// </returns>
        private StackLayout GetColorSelector() {
            this._colorList = new RadioButtonList() {
                Items = {
                    "Default",
                    "Redscale",
                },
                ToolTip = "Select a color palette"
            };

            this._colorList.SelectedKey = "Default";
            this._colorList.SelectedIndexChanged += this.OnColorSelectorChanged;
            
            return new StackLayout() {
                Items = {
                    "Color palettes",
                    this._colorList,
                },
                Padding = new Padding() {
                    Bottom = 10,
                    Top = 10,
                }
            };
        }

        /// <summary>
        /// Get the prefab selector layout
        /// </summary>
        /// <returns>
        /// The prefab selector
        /// </returns>
        private StackLayout GetPrefabSelectorLayout() {
            // Preconfigured list of options
            this._prefabList = new RadioButtonList() {
                Items = {
                    "A",
                    "B",
                    "C"
                },
                ToolTip = "Select preconfigured parameters"
            };

            this._prefabList.SelectedKey = "A";
            this._prefabList.SelectedIndexChanged += this.OnPrefabListChanged;

            return new StackLayout() {
                Items = {
                    "Prefabs",
                    this._prefabList,
                },
                Padding = new Padding() {
                    Bottom = 10,
                    Top = 10,
                }
            };
        } 
        
        /// <summary>
        /// Get the 'GO' button layout
        /// </summary>
        /// <returns>
        /// The 'GO' button layout
        /// </returns>
        private StackLayout GetGoButton() {
            Button goBtn = new Button {
                Width = 400,
                Text = "Go",
            };
            goBtn.Click += this.OnGoButtonClicked;
            
            // The 'GO' button
            return new StackLayout() {
                Items = {
                    goBtn
                },
                Padding = new Padding() {
                    Bottom = 10,
                }
            };
        }

        private void OnColorSelectorChanged(object? sender, EventArgs args) {
            // colorList is not null at this point
            switch (this._colorList!.SelectedKey) {
                case "Default":
                    this._mandelView.ColorPalette = Mandelbrot.ColorPaletteDefault;
                    break;
                case "Redscale":
                    this._mandelView.ColorPalette = Mandelbrot.ColorPaletteRedscale;
                    break;
                default:
                    throw new InvalidDataException($"Invalid color palette key {this._colorList.SelectedKey}");
            }

            this._mandelView.Invalidate();
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
            switch (this._prefabList!.SelectedKey) {
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
                    throw new InvalidDataException($"Invalid prefab key {this._prefabList.SelectedKey}");
            }

            this._middleXField.Text = cx.ToString();
            this._middleYField.Text = cy.ToString();
            this._scaleField.Text = scale.ToString(Thread.CurrentThread.CurrentCulture);
            
            this.UpdateMandelViewXY(cx, cy);
            this._mandelView.Scale = scale;
            this._mandelView.Invalidate();
        }
        
        /// <summary>
        /// Update the X and Y coordinates of the mandelbrot viewer.
        /// Note that this does not rerender the mandelbrot 
        /// </summary>
        /// <param name="centerX">The new center X coordinate</param>
        /// <param name="centerY">The new center Y coordinate</param>
        // ReSharper disable once InconsistentNaming
        private void UpdateMandelViewXY(int centerX, int centerY) {
            this._mandelView.CX = centerX;
            this._mandelView.CY = centerY;
        }
        
        /// <summary>
        /// Called when the user presses a mouse button within the mandelbrot viewer.
        /// This function will update the X and Y coordinate of the mandelbrot to the user's mouse position,
        /// and adjust the scale of the mandelbrot.
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="args">The arguments to this event</param>
        private void OnMandelViewMouseDown(object? sender, MouseEventArgs args) {
            this.ApplyZoom(args.Buttons);

            double normalMouseX = args.Location.X * 4 / this._mandelView.Width; 
            double normalMouseY = args.Location.Y * 4 / this._mandelView.Height;

            double centeredMouseX = normalMouseX - 2.0;
            double centeredMouseY = normalMouseY - 2.0;

            this._mandelView.CX = centeredMouseX;
            this._mandelView.CY = centeredMouseY;
            
            DateTimeOffset initialMouseDown = DateTimeOffset.Now;
            this._isMouseDown = true;
            Thread t = new Thread(() => {
                while (this._isMouseDown) {
                    long deltaMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - initialMouseDown.ToUnixTimeMilliseconds();
                    if (MouseDownConstantZoomMinimumDurationMs > deltaMillis) {
                        continue;
                    }
                
                    Application.Instance.Invoke(() => {
                        this.ApplyZoom(args.Buttons);
                        this._mandelView.Invalidate();
                    });
                    initialMouseDown = DateTimeOffset.Now;
                }
            });
            t.Start();

            this._middleXField.Text = this._mandelView.CX.ToString(Thread.CurrentThread.CurrentCulture);
            this._middleYField.Text = this._mandelView.CY.ToString(Thread.CurrentThread.CurrentCulture);
            
            this._mandelView.Invalidate();
        }

        /// <summary>
        /// Apply zoom to the mandelbrot viewer based on the mouse button pressed.
        /// Note that this does not rerender the mandelbrot
        /// </summary>
        /// <param name="buttons">The mouse buttons pressed</param>
        private void ApplyZoom(MouseButtons buttons) {
            // Left = zoom in
            // Right = zoom out
            switch (buttons) {
                case MouseButtons.Primary:
                    this._mandelView.Scale -= this._mandelView.Scale * ScaleStepFactor;
                    break;
                case MouseButtons.Alternate:
                    this._mandelView.Scale += this._mandelView.Scale * ScaleStepFactor;
                    break;
            }
            
            this._scaleField.Text = this._mandelView.Scale.ToString(Thread.CurrentThread.CurrentCulture);
        }
        
        /// <summary>
        /// Called when the user stops pressing their mouse button when inside of the mandelbrot viewer
        /// </summary>
        /// <param name="sender">The object which triggered this event</param>
        /// <param name="args">Arguments to the event</param>
        private void OnMandelViewMouseUp(object? sender, MouseEventArgs args) {
            this._isMouseDown = false;
        }

        /// <summary>
        /// This event is called when the 'GO' button is clicked.
        /// This event handler will draw the mandelbrot according to the provided parameters
        /// </summary>
        /// <param name="sender">The object which triggered this event</param>
        /// <param name="args">Arguments to the event</param>
        private void OnGoButtonClicked(object? sender, EventArgs args) {
            this._mandelView.Scale = double.Parse(this._scaleField.Text);
            this._mandelView.Limit = int.Parse(this._limitField.Text);
                
            int centerX = int.Parse(this._middleXField.Text);
            int centerY = int.Parse(this._middleYField.Text);
            this._mandelView.Width = int.Parse(this._widthField.Text);
            this._mandelView.Height = int.Parse(this._heightField.Text);
            
            this.UpdateMandelViewXY(centerX, centerY);    
            
            this._mandelView.Ready = true;
            this._mandelView.Invalidate();
        }

        /// <summary>
        /// Calculate the minimum and maximum coordinate for a provided center and size
        /// </summary>
        /// <param name="center">The center</param>
        /// <param name="size">The size</param>
        /// <returns>
        /// The min and max coordinates
        /// </returns>
        private static (int, int) CenterToBounds(int center, int size) {
            int min = center - size / 2;
            int max = center + size / 2;
            return (min, max);
        }
        
        /// <summary>
        /// Build a TableRow with a label and a TextBox
        /// </summary>
        /// <param name="label">The text which should be put in the label</param>
        /// <returns>
        /// A table row consisting of a label on the left and a TextBox on the right
        /// </returns>
        private static KeyValuePair<TableCell[], TextBox> BuildLabelledInputRow(string label) {
            TextBox box = new TextBox();
            TableCell[] cells = new TableCell[] {
                new TableCell {
                    Control = new Label {
                        Width = 200,
                        Text = label
                    }
                },
                new TableCell {
                    Control = box
                }
            };

            return new KeyValuePair<TableCell[], TextBox>(cells, box);
        }
    }
}