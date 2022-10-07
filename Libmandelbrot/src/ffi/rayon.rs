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
    cx: f64,
    cy: f64,
    width: i32,
    height: i32,
    limit: i32,
    scale: f64,
    color_palette: i32,
    result: *mut i32,
){
    let raw_ptr = SyncSendRawPtr(result);

    (0..width).into_par_iter()
        .for_each(|px| (0..height).into_iter()
            .for_each(|py| {
                let npx = px as f64 * 4.0 / width as f64;
                let npy = py as f64 * 4.0 / height as f64;

                let x = (npx + cx - 2.0) * scale;
                let y = (npy + cy - 2.0) * scale;

                let mut a = 0f64;
                let mut b = 0f64;

                let mut iteration = 0;
                loop {
                    let tmp_a = a * a - b * b + x;
                    b = 2f64 * a * b + y;
                    a = tmp_a;
                    iteration += 1;

                    if distance_squared(a, b, 0f64, 0f64) > 4.0 {
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

fn distance_squared(xa: f64, ya: f64, xb: f64, yb: f64) -> f64 {
    (xa * xa + ya * ya) - (xb * xb + yb * yb)
}