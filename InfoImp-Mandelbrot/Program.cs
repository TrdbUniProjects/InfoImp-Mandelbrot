using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot {
    public class MainForm : Form {
        private const int MandelWidth = 400;
        private const int MandelHeight = 400;

        private readonly MandelView _mandelView;
        private readonly TextBox _middleXField, _middleYField, _scaleField, _limitField;
        
        public MainForm() {
            this.Title = "INFOIMPL Mandelbrot";
            base.Size = new Size(MandelWidth + 40, MandelHeight + 40 + 250);
            KeyValuePair<TableRow, TextBox> middleXField = BuildLabelledInputRow("Midden X");
            KeyValuePair<TableRow, TextBox> middleYField = BuildLabelledInputRow("Midden Y");
            KeyValuePair<TableRow, TextBox> scaleField = BuildLabelledInputRow("Schaal");
            KeyValuePair<TableRow, TextBox> maxCountField = BuildLabelledInputRow("Max Aantal");
            
            // Default values
            middleXField.Value.Text = "0";
            middleYField.Value.Text = "0";
            scaleField.Value.Text = "1";
            maxCountField.Value.Text = "100";

            this._middleXField = middleXField.Value;
            this._middleYField = middleYField.Value;
            this._scaleField = scaleField.Value;
            this._limitField = maxCountField.Value;

            Button goBtn = new Button {
                Width = 400,
                Text = "Go",
            };

            goBtn.Click += this.OnGoButtonClicked;
            
            this._mandelView = new MandelView() { Size = new Size(MandelWidth, MandelHeight) };
            this.Content = new StackLayout {
                Items = {
                    new TableLayout {
                        Spacing = new Size(0, 10),
                        Rows = {
                            middleXField.Key,
                            middleYField.Key,
                            scaleField.Key,
                            maxCountField.Key,
                        }
                    },
                    goBtn,
                    this._mandelView
                }
            };

            this.Padding = new Padding(20);
        }

        private void OnGoButtonClicked(object? sender, EventArgs args) {
            this._mandelView.Scale = double.Parse(this._scaleField.Text);
            this._mandelView.Limit = int.Parse(this._limitField.Text);
                
            int centerX = int.Parse(this._middleXField.Text);
            this._mandelView.XMin = centerX - (MandelWidth / 2);
            this._mandelView.XMax = centerX + (MandelWidth / 2);

            int centerY = int.Parse(this._middleYField.Text);
            this._mandelView.YMin = centerY - (MandelHeight / 2);
            this._mandelView.YMax = centerY + (MandelHeight / 2);
                
            this._mandelView.Ready = true;
            this._mandelView.Invalidate();
        }

        private static KeyValuePair<TableRow, TextBox> BuildLabelledInputRow(string label) {
            TextBox box = new TextBox();
            TableRow row = new TableRow {
                Cells = {
                    new TableCell {
                        Control = new Label {
                            Width = 200,
                            Text = label
                        }
                    },
                    new TableCell {
                        Control = box
                    }
                }
            };

            return new KeyValuePair<TableRow, TextBox>(row, box);
        }
    }
}