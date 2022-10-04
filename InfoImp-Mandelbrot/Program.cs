using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot {
    public class MainForm : Form {
        private const int MandelWidth = 400;
        private const int MandelHeight = 400;
        
        public MainForm() {
            this.Title = "INFOIMPL Mandelbrot";
            this.Size = new Size(MandelWidth + 40, MandelHeight + 40 + 250);
            
            KeyValuePair<TableRow, TextBox> middleXField = LabelledInputRow("Midden X");
            KeyValuePair<TableRow, TextBox> middleYField = LabelledInputRow("Midden Y");
            KeyValuePair<TableRow, TextBox> scaleField = LabelledInputRow("Schaal");
            KeyValuePair<TableRow, TextBox> maxCountField = LabelledInputRow("Max Aantal");
            
            // Default values
            middleXField.Value.Text = "0";
            middleYField.Value.Text = "0";
            scaleField.Value.Text = "1";
            maxCountField.Value.Text = "100";
            
            Button goBtn = new Button {
                Width = 400,
                Text = "Foo",
            };

            Bitmap mandelbrotBitmap = new Bitmap(MandelWidth, MandelHeight, PixelFormat.Format24bppRgb);
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
                    mandelbrotBitmap,
                }
            }; 
            
            goBtn.Click += (_, _) => {

                int centerX = int.Parse(middleXField.Value.Text);
                int centerY = int.Parse(middleYField.Value.Text);
                double scale = double.Parse(scaleField.Value.Text);
                int limit = int.Parse(maxCountField.Value.Text);

                int xMin = centerX - (MandelWidth / 2) + centerX;
                int xMax = centerX + (MandelWidth / 2) + centerX;

                Console.WriteLine($"min: {xMin} max: {xMax}");
                
                int yMin = centerY - (MandelHeight / 2) + centerY;
                int yMax = centerY + (MandelHeight / 2) + centerY;

                Console.WriteLine("Generating...");
                int[,] mandelbrotSet = Mandelbrot.CalculateMandelbrotSet(xMin, xMax, yMin, yMax, limit, scale);
                BitmapData lok = mandelbrotBitmap.Lock();
                for (int x = 0; x < MandelWidth; x++) {
                    for (int y = 0; y < MandelHeight; y++) {
                        Color c = Color.FromArgb(mandelbrotSet[x, y]);
                        lok.SetPixel(x, y, c);
                    }
                }
                
                lok.Dispose();
                
                Console.WriteLine("Mandelset has been drawn");
            };

            this.Padding = new Padding(20);

        }

        private static KeyValuePair<TableRow, TextBox> LabelledInputRow(string label) {
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