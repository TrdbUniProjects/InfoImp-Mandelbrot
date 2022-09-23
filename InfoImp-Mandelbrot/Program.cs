using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot {
    public class MainForm : Form {

        public MainForm() {
            this.Title = "INFOIMPL Mandelbrot";
            this.Size = new Size(440, 440 + 250);

            KeyValuePair<TableRow, TextBox> middleX = this.LabelledInputRow("Midden X");
            KeyValuePair<TableRow, TextBox> middleY = this.LabelledInputRow("Midden Y");
            KeyValuePair<TableRow, TextBox> scale = this.LabelledInputRow("Schaal");
            KeyValuePair<TableRow, TextBox> maxCount = this.LabelledInputRow("Max Aantal");
            
            Bitmap bitmap = new Bitmap(400, 400, PixelFormat.Format24bppRgb);
            Graphics graphics = new Graphics(bitmap);
            
            Button goBtn = new Button {
                Width = 400,
                Text = "Foo",
            };
            goBtn.Click += (_, _) => {
                ThreadStart work = () => {
                    Console.WriteLine("Drawing mandelbrot!");
                    PaintMandelbrot(graphics, 0, 0, 0.05, 50);
                };
                Thread thread = new Thread(work);
                thread.Start();
            };

            this.Padding = new Padding(20);
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
                    bitmap,
                }
            }; 
        }

        void PaintMandelbrot(Graphics g, int cx, int cy, double scale, int limit) {
            for (int x = 0; x <= 400; x++) {
                for (int y = 0; y <= 400; y++) {
                    int mandelbrot = this.CalcMandelbrotForPoint(x, y, cx, cy, scale, limit);
                    Console.WriteLine(mandelbrot);
                    
                    if (mandelbrot % 2 == 0) {
                        g.FillRectangle(new SolidBrush(Colors.Black), x, y, 1, 1);
                    } else {
                        g.FillRectangle(new SolidBrush(Colors.White), x, y, 1, 1);
                    }
                    
                    g.Flush();
                }
            }
            
            Console.WriteLine("Done!");
        }
        
        int CalcMandelbrotForPoint(int cx, int cy, int x, int y, double scale, int limit) {
            int mandelX = cx;
            int mandelY = cy;

            int idx;
            for (idx = 0; Distance(mandelX, mandelY, cx, cy) < scale && idx < limit; idx++) {
                mandelX = mandelX * mandelX - mandelY * mandelY + x;
                mandelY = 2 * mandelX * mandelY + y;
            }

            return idx;
        }

        int Distance(int xa, int ya, int xb, int yb) {
            return (int) Math.Round(
                Math.Sqrt(
                    (Math.Pow(xa, 2) + Math.Pow(ya, 2)) - (Math.Pow(xb, 2) + Math.Pow(yb, 2))
                )
            );
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