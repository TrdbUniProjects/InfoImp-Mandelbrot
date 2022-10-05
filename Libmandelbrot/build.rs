fn main() {
    println!("cargo:rerun-if-changed=ffi/ocl.cl");
}