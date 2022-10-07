use std::time::Instant;
use ocl::ProQue;
use crate::ffi::{compact_rgb, get_color_palette_len, get_default_color_palette, get_redscale_color_palette};
use lazy_static::lazy_static;
use ocl::ffi::libc::exit;

const OCL_SHADER: &str = include_str!("./ocl.cl");

lazy_static! {
    // Compile the kernel as soon as the program loads, also avoid recompiling every time
    static ref PRO_QUE: ProQue = ProQue::builder().src(OCL_SHADER).build().unwrap();
}

#[no_mangle]
extern "C" fn ocl_calculate_mandelbrot_set(
    cx: f64,
    cy: f64,
    width: i32,
    height: i32,
    limit: i32,
    scale: f64,
    color_palette: i32,
    result: *mut i32
) -> i64 {
    let now = Instant::now();
    let palette_len = get_color_palette_len(color_palette);

    let scalar = 1.0 / scale;

    // Create the buffer and configure the kernel to use
    let mut pro_que = PRO_QUE.clone();
    pro_que.set_dims((width, height));
    let buffer = pro_que.create_buffer::<u32>().unwrap();
    let mut kernel = pro_que
        .kernel_builder("mandelbrot");
    kernel.arg(&buffer);

    // Gather extensions
    let ocl_platform = ocl::Platform::first().unwrap();
    let extensions = ocl_platform.extensions().unwrap();
    let extensions = extensions.iter().collect::<Vec<_>>();

    // If f64 support is present, use it
    if extensions.contains(&"cl_khr_fp64") || extensions.contains(&"cl_amd_fp64") {
        println!("OCL supports f64, using that");
        kernel
            .arg(cx as f64)
            .arg(cy as f64)
            .arg(width)
            .arg(height)
            .arg(limit as u32)
            .arg(scalar as f64);
    } else {
        kernel
            .arg(cx as f32)
            .arg(cy as f32)
            .arg(width)
            .arg(height)
            .arg(limit as u32)
            .arg(scalar as f32);
    }

    // Configure the rest of the kernel and build it
    kernel.arg(palette_len as u32);
    let kernel = kernel.build().unwrap();

    // Run the kernel
    unsafe { kernel.enq().unwrap() };

    let buffer_len = width * height;

    // Copy the results back
    // TODO can we elide the allocation here?
    let mut local_buffer = vec![0u32; buffer_len as usize];
    buffer.read(&mut local_buffer).enq().unwrap();

    // Copy the results buffer into the results pointer with the resolved color
    for idx in 0..(buffer_len as usize) {
        unsafe { *result.offset(idx as isize) = get_color_at_idx(color_palette, local_buffer[idx] as usize) };
    }

    now.elapsed().as_nanos() as i64
}

fn get_color_at_idx(palette: i32, idx: usize) -> i32 {
    let (r, g, b) = match palette {
        0 => get_default_color_palette().get(idx).unwrap().clone(),
        1 => get_redscale_color_palette().get(idx).unwrap().clone(),
        _ => panic!("Unknown color palette {palette}")
    };

    compact_rgb(r, g, b)
}