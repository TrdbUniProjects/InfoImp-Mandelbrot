use std::time::Instant;
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
) -> i64 {
    let raw_ptr = SyncSendRawPtr(result);

    let now = Instant::now();
    let zoom_scalar = 1.0 / scale;
    (0..width).into_par_iter()
        .for_each(|px| {
            let image_space_x = px as f64 / width as f64 * 4.0 - 2.0;
            let x = cx + image_space_x * zoom_scalar;

            (0..height).into_iter()
                .for_each(|py| {
                    let image_space_y = py as f64 / height as f64 * 4.0 - 2.0;
                    let y = cy + image_space_y * zoom_scalar;

                    let mut a = 0f64;
                    let mut b = 0f64;

                    let mut iteration = 0;
                    loop {
                        let a_squared = a * a;
                        let b_squared = b * b;

                        let tmp_a = a_squared - b_squared + x;
                        b = 2f64 * a * b + y;
                        a = tmp_a;
                        iteration += 1;

                        if a_squared + b_squared > 4.0 {
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
        });

    now.elapsed().as_nanos() as i64
}