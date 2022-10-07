using System.Runtime.InteropServices;

namespace InfoImp_Mandelbrot; 

public enum Platform {
    CSharp,
    Rust,
    RustOcl
}

static class PlatformExtension {
    public static Platform[] GetAvailablePlatforms() {
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

        // TODO Check for OpenCL
        // I dont think we can figure out if OpenCL is installed
        // without checking every possible path.
        // Essentially we need what DllImport is doing as a function
    
        if (osSupported && archSupported) {
            return new [] { Platform.CSharp, Platform.Rust, Platform.RustOcl };
        }

        return new [] { Platform.CSharp };
    }     
}