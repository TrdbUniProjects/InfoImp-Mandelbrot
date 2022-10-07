using Eto.Drawing;
using Eto.Forms;

namespace InfoImp_Mandelbrot; 

public class Menu {
    
    public static MenuBar GetMenuBar(Control parent) {
        return new MenuBar() {
            QuitItem = GetQuitCommand(),
            AboutItem = GetAboutCommand(parent)
        };
    }

    private static Command GetAboutCommand(Control parent) {
        return new Command((_, _) => GetAboutDialog().ShowModal(parent)) {
            MenuText = "About"
        };
    }

    private static Command GetQuitCommand() {
        return new Command((_, _) => Application.Instance.Quit()) {
            MenuText = "Quit"
        };
    }

    private static Dialog GetAboutDialog() {
        return new Dialog {
            Content = new Label {
                Text = "Mandelbrot Renderer - (C) 2022 - Jesse Roos and Tobias de Bruijn.  MIT licensed\n" +
                       "\n" +
                       "This renderer supports three backends: C#, Rust and GPU. Every backend has a different purpose:\n" +
                       "- C#: The C# backend is supported anywhere that this application will run. However, it is comparatively slow.\n" +
                       "- Rust: The Rust backend is only supported on x86_64 Windows and Linux. It is decently fast and can render at high resolutions.\n" +
                       "- GPU: The GPU backend uses OpenCL and Rust, thus, like Rust it only runs on x86_64 Windows and Linux. Furthermore, it requires support from the GPU driver. It is incredibly fast and can do high resolutions, if the driver supports it.\n" +
                       "For more information about setting up the GPU backend, refer to the README in the repository of this project.\n" +
                       "\n" +
                       "When chosing a limit and size it is important to keep in mind the backend used. If a high resolution or high limit is chosen, the C# backend must not be chosen.\n" +
                       "It becomes too slow to be usable. The Rust or GPU backends are then preferred. When wanting to render in very high detail, but care a little less about speed, use the Rust backend.\n" +
                       "If your platform supports the fp64 extension for OCL, you can also render in very high detail on the GPU. If it does not, it can only use f32 precision.\n" +
                       "Lastly, while the GPU is very fast, copying the data to it and running the code is not. E.g. a size 1000 image at limit 1000000 takes 37ms with the Rust backend, and 32ms with the GPU backend. (Ryzen 5900X + NVidia RTX 2080Ti) \n " +
                       "On the contrary, a size 8000 image with scale 1000000 takes 2.4 seconds in the Rust backend, but only 1.2 seconds in the GPU backend. The C# backend takes at least 2 minutes (patience ran out :) ).\n" +
                       "When running at lower settings, it usually is better to use the C# backend though. This is due to the overhead in calling C functions (The Rust backend uses the C ABI) and in running code on the GPU.\n" +
                       "\n" +
                       "Made for the bachelor Informatica at Utrecht University.\n" +
                       "\n" +
                       "OpenCL is a trademark of Apple Inc. used by permission by Khronos."
            },
            ClientSize = new Size(700, 525),
        };
    }

}