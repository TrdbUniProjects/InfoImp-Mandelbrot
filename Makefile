all:

.PHONY: build-windows build-linux
build-windows: lib-windows net-windows
build-linux: lib-linux net-linux

.PHONY: lib-linux lib-windows
lib-linux: Libmandelbrot/target/x86_64-unknown-linux-gnu/release/libmandelbrot_rayon.so Libmandelbrot/target/x86_64-unknown-linux-gnu/release/libmandelbrot_ocl.so
lib-windows: Libmandelbrot/target/x86_64-pc-windows-msvc/release/mandelbrot_ocl.dll Libmandelbrot/target/x86_64-pc-windows-msvc/release/mandelbrot_rayon.dll

Libmandelbrot/target/x86_64-pc-windows-msvc/release/mandelbrot_rayon.dll:
	cd Libmandelbrot && $(MAKE) build-rayon-windows

Libmandelbrot/target/x86_64-pc-windows-msvc/release/mandelbrot_ocl.dll:
	cd Libmandelbrot && $(MAKE) build-ocl-windows

Libmandelbrot/target/x86_64-unknown-linux-gnu/release/libmandelbrot_rayon.so:
	cd Libmandelbrot && $(MAKE) build-rayon-linux

Libmandelbrot/target/x86_64-unknown-linux-gnu/release/libmandelbrot_ocl.so:
	cd Libmandelbrot && $(MAKE) build-ocl-linux

.PHONY: net-linux net-windows
net-linux: InfoImp-Mandelbrot.Gtk/bin/Release/net6.0/linux-x64/InfoImp-Mandelbrot.Gtk
net-windows: InfoImp-Mandelbrot.Wpf/bin/Release/net6.0/linux-x64/InfoImp-Mandelbrot.Wpf

InfoImp-Mandelbrot.Gtk/bin/Release/net6.0/linux-x64/InfoImp-Mandelbrot.Gtk: lib-linux
	cd InfoImp-Mandelbrot.Gtk && \
		dotnet build --sc -r linux-x64

InfoImp-Mandelbrot.Wpf/bin/Release/net6.0/linux-x64/InfoImp-Mandelbrot.Wpf: lib-windows
	cd InfoImp-Mandelbrot.Wpf && \
		dotnet build --sc -r win10-x64
