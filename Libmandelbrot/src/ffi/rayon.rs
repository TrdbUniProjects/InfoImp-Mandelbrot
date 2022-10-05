use rayon::prelude::*;
use crate::ffi::get_color;

#[derive(Clone, Copy)]
struct SyncSendRawPtr<T>(*mut T);

unsafe impl<T> Send for SyncSendRawPtr<T> {}
unsafe impl<T> Sync for SyncSendRawPtr<T> {}

impl<T> SyncSendRawPtr<T> {
    unsafe fn set_value_at_offset(self, value: T, offset: isize) {
        *self.0.offset(offset) = value;
    }
}

#[no_mangle]
pub extern "C" fn calculate_mandelbrot_set(
    xmin: i32,
    xmax: i32,
    ymin: i32,
    ymax: i32,
    limit: i32,
    scale: f64,
    color_palette: i32,
    result: *mut i32,
){
    let width = xmax - xmin;
    let height = ymax - ymin;
    let distance_limit = scale * ((width + height) as f64) / 2f64;

    let raw_ptr = SyncSendRawPtr(result);

    (0..width).into_par_iter()
        .for_each(|px| (0..height).into_iter()
            .for_each(|py| {
                let x: f64 = (xmin as f64) * scale + scale * (px as f64);
                let y: f64 = (ymin as f64) * scale + scale * (py as f64);

                let mut a = 0f64;
                let mut b = 0f64;

                let mut iteration = 0;
                loop {
                    let tmp_a = a * a - b * b + x;
                    b = 2f64 * a * b + y;
                    a = tmp_a;
                    iteration += 1;

                    if distance(a, b, 0f64, 0f64) > distance_limit {
                        break;
                    }

                    if iteration >= limit {
                        break;
                    }
                }

                unsafe {
                    // We cant use the *mut i32 directly, because it may
                    // not be moved accross thread by default
                    // however it is perfectly fine here.
                    // SAFETY: The pointer at the specified offset is exclusive to us here.
                    raw_ptr.set_value_at_offset(get_color(iteration, a, b, color_palette), (width * px + py) as isize);
                }
            })
        )
}

fn distance(xa: f64, ya: f64, xb: f64, yb: f64) -> f64 {
    ((xa * xa + ya * ya) - (xb * xb + yb * yb)).sqrt()
}