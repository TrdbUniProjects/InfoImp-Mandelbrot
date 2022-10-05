use ocl::ProQue;
use crate::ffi::{compact_rgb, get_color_palette_len, get_default_color_palette, get_redscale_color_palette};
use lazy_static::lazy_static;

const OCL_SHADER: &str = include_str!("./ocl.cl");

lazy_static! {
    // Compile the kernel as soon as the program loads, also avoid recompiling every time
    static ref PRO_QUE: ProQue = ProQue::builder().src(OCL_SHADER).build().unwrap();
}

#[no_mangle]
extern "C" fn ocl_calculate_mandelbrot_set(
    xmin: i32,
    xmax: i32,
    ymin: i32,
    ymax: i32,
    limit: i32,
    scale: f64,
    color_palette: i32,
    result: *mut i32
) {
    let width = xmax - xmin;
    let height = ymax - ymin;
    let palette_len = get_color_palette_len(color_palette);

    let mut pro_que = PRO_QUE.clone();
    pro_que.set_dims((width, height));

    let buffer = pro_que.create_buffer::<u32>().unwrap();
    let kernel = pro_que
        .kernel_builder("mandelbrot")
        .arg(&buffer)
        .arg(xmin)
        .arg(xmax)
        .arg(ymin)
        .arg(ymin)
        .arg(limit as u32)
        .arg(scale as f32)
        .arg(palette_len as u32)
        .build()
        .unwrap();

    unsafe { kernel.enq().unwrap() };

    let buffer_len = width * height;

    let mut local_buffer = vec![0u32; buffer_len as usize];
    buffer.read(&mut local_buffer).enq().unwrap();

    for idx in 0..(buffer_len as usize) {
        unsafe { *result.offset(idx as isize) = get_color_at_idx(color_palette, local_buffer[idx] as usize) };
    }
}

fn get_color_at_idx(palette: i32, idx: usize) -> i32 {
    let (r, g, b) = match palette {
        0 => get_default_color_palette().get(idx).unwrap().clone(),
        1 => get_redscale_color_palette().get(idx).unwrap().clone(),
        _ => panic!("Unknown color palette {palette}")
    };

    compact_rgb(r, g, b)
}