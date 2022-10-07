all: 

SRC = $(shell find -type f -name \*.rs -or -name \*.cl -or -name Cargo.toml)

.PHONY: build-windows build-linux
build-windows: build-ocl-windows build-rayon-windows
build-linux: build-ocl-linux build-rayon-linux

.PHONY: build-ocl-windows build-ocl-linux
build-ocl-windows: target/x86_64-pc-windows-msvc/release/mandelbrot_ocl.dll
build-ocl-linux: target/x86_64-unknown-linux-gnu/release/libmandelbrot_ocl.so

.PHONY: build-rayon-windows build-rayon-linux
build-rayon-windows: target/x86_64-pc-windows-msvc/release/mandelbrot_rayon.dll
build-rayon-linux: target/x86_64-unknown-linux-gnu/release/libmandelbrot_rayon.so

.PHONY: libopencl-linux
libopencl-linux:
	sudo apt install -qq -y ocl-icd-opencl-dev

target/x86_64-pc-windows-msvc/release/mandelbrot_ocl.dll: $(SRC)
	cargo build --release --target x86_64-pc-windows-msvc --features ocl
	copy target\x86_64-pc-windows-msvc\release\mandelbrot.dll target\x86_64-pc-windows-msvc\release\mandelbrot_ocl.dll

target/x86_64-unknown-linux-gnu/release/libmandelbrot_ocl.so: $(SRC) libopencl-linux
	cargo build --release --target x86_64-unknown-linux-gnu --features ocl
	cp target/x86_64-unknown-linux-gnu/release/libmandelbrot.so target/x86_64-unknown-linux-gnu/release/libmandelbrot_ocl.so

target/x86_64-pc-windows-msvc/release/mandelbrot_rayon.dll: $(SRC)
	cargo build --release --target x86_64-pc-windows-msvc --features rayon
	copy target\x86_64-pc-windows-msvc\release\mandelbrot.dll target\x86_64-pc-windows-msvc\release\mandelbrot_rayon.dll

target/x86_64-unknown-linux-gnu/release/libmandelbrot_rayon.so: $(SRC)
	cargo build --release --target x86_64-unknown-linux-gnu --features rayon
	cp target/x86_64-unknown-linux-gnu/release/libmandelbrot.so target/x86_64-unknown-linux-gnu/release/libmandelbrot_rayon.so