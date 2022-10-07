# Mandelbrot calculations
Calculate and display the mandelbrot set

## Backends
- **C#** Calculates the set on the CPU sequentially in C#
- **Rust with Rayon** Calculate the set on the CPU in parallel in Rust
>Note: Supported on x86_64 Windows and Linux
- **Rust with OCL** Calculate the set on the GPU in parallel
>Note: Supported on x86_64 Windows and Linux. Requires libopencl to be installed on the PATH.
> On Ubuntu this requires the following package: `ocl-icd-opencl-dev`. Besides this,
> the GPU driver needs to support OpenCL as well. Nvidia and AMD's drivers do this by default.
> For Intel this requires installing `intel-opencl-icd` [More information](https://github.com/intel/compute-runtime/blob/master/opencl/doc/DISTRIBUTIONS.md)
> 
> For Windows the OpenCL library is included in the Windows package, however the GPU driver must support it also.
> For intel: [read more](https://www.intel.com/content/www/us/en/developer/articles/tool/opencl-drivers.html#proc-graph-section)

## Building
Building this full project requires the following dependencies:
- DotNET
- Rust toolchain
- OpenCL
- GNU make
>Note: On Windows this can be installed via [Chocolatey](https://chocolatey.org/install#individual): `choco install make`

Then you can run `make build-linux` or `make build-windows` in the repository root.

## Running
After building, run `dotnet run --project InfoImp-Mandelbrot.Gtk` (Linux) or `dotnet run --project InfoImp-Mandelbrot.Wpf` (Windows).