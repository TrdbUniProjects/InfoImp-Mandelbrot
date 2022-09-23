using Eto.Forms;

namespace InfoImp_Mandelbrot {
    public class MainForm : Form {

        public MainForm() {
            this.Title = "INFOIMPL Mandelbrot";
        }
        public static void Main(String[] args) {
            new Application().Run(new MainForm());
        }
    }
}