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
            
            KeyValuePair<TableRow, TextBox> middleX = this.LabelledInputRow("Midden X");
            KeyValuePair<TableRow, TextBox> middleY = this.LabelledInputRow("Midden Y");
            KeyValuePair<TableRow, TextBox> scale = this.LabelledInputRow("Schaal");
            KeyValuePair<TableRow, TextBox> maxCount = this.LabelledInputRow("Max Aantal");
            
            middleX.Value.Text = "0";
            middleY.Value.Text = "0";
            scale.Value.Text = "5";
            maxCount.Value.Text = "100";
            
            
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
                            middleX.Key,
                            middleY.Key,
                            scale.Key,
                            maxCount.Key,
                        }
                    },
                    goBtn,
                    mandelbrotBitmap,
                }
            }; 
            
            goBtn.Click += (_, _) => {

                int cMiddleX = int.Parse(middleX.Value.Text);
                int cMiddleY = int.Parse(middleY.Value.Text);
                double mandelScale = double.Parse(scale.Value.Text);
                int mandelLimit = int.Parse(maxCount.Value.Text);
                
                // We generate the mandelbrot in a new thread to avoid
                // stalling the main UI
                ThreadStart work = () => {
                    Console.WriteLine("Drawing mandelbrot!");

                    Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();

                 /*   int[,] mandelSet = Mandelbrot.GenerateMandelbrot(
                        mandelLimit,
                        new Size(MandelWidth, MandelHeight),
                        cMiddleX - mandelScale / 2D,
                        cMiddleX + mandelScale / 2D,
                        cMiddleY - mandelScale / 2D,
                        cMiddleY + mandelScale / 2D,
                        mandelScale
                    );
                   */

                 int[,] mandelSet = Mandelbrot.GenerateMandelbrotV2(
                     cMiddleX,
                     cMiddleY,
                     mandelScale,
                     mandelLimit,
                     400
                 );
                    
                    timer.Stop();
                    Console.WriteLine($"Generating mandelbrot done. Took {timer.ElapsedMilliseconds} ms");

                    // We set the bitmap pixel's seperately, to allow pure benchmarking
                    // of the generator function
                    Bitmap bitmap = new Bitmap(MandelWidth, MandelHeight, PixelFormat.Format24bppRgb);
                    for (int x = 0; x < MandelWidth; x++) {
                        for (int y = 0; y < MandelHeight; y++) {
                            // TODO use pixel pointer directly for a speedup
                            bitmap.SetPixel(x, y, Color.FromArgb(mandelSet[x, y]));
                        }
                    }

                    Application.Instance.Invoke(() => {
                        StackLayout layout = (StackLayout)this.Content;
                        layout.Items.RemoveAt(layout.Items.Count - 1);
                        layout.Items.Add(bitmap);
                    });
                };
                Thread thread = new Thread(work);
                thread.Start();
            };

            this.Padding = new Padding(20);

        }

        private KeyValuePair<TableRow, TextBox> LabelledInputRow(string label) {
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