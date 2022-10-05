use rayon::prelude::*;

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

fn get_color(iteration: i32, a: f64, b: f64, color_palette: i32) -> i32 {
    match color_palette {
        0 => get_color_with_palette(iteration, a, b, get_default_color_palette()),
        1 => get_color_with_palette(iteration, a, b, get_redscale_color_palette()),
        _ => panic!("Invalid color palette {color_palette}")
    }
}

const fn get_default_color_palette() -> [(u8, u8, u8); 16] {
    [
        (66, 30, 15),
        (25, 7, 26),
        (9, 1, 47),
        (4, 4, 73),
        (8, 7, 100),
        (12, 44, 138),
        (24, 82, 177),
        (57, 125, 209),
        (134, 181, 229),
        (211, 236, 248),
        (241, 233, 191),
        (248, 201, 95),
        (255, 170, 0),
        (204, 128, 0),
        (153, 87, 0),
        (106, 52, 3)
    ]
}

const fn get_redscale_color_palette() -> [(u8, u8, u8); 25] {
    [
        (10, 0, 0),
        (20, 0, 0),
        (30, 0, 0),
        (40, 0, 0),
        (50, 0, 0),
        (60, 0, 0),
        (70, 0, 0),
        (80, 0, 0),
        (90, 0, 0),
        (100, 0, 0),
        (110, 0, 0),
        (120, 0, 0),
        (130, 0, 0),
        (140, 0, 0),
        (150, 0, 0),
        (160, 0, 0),
        (170, 0, 0),
        (180, 0, 0),
        (190, 0, 0),
        (200, 0, 0),
        (210, 0, 0),
        (220, 0, 0),
        (230, 0, 0),
        (240, 0, 0),
        (250, 0, 0)
    ]
}

fn get_color_with_palette<const N: usize>(iteration: i32, a: f64, b: f64, palette: [(u8, u8, u8); N]) -> i32 {
    let smoothed: f64 = ((a * a + b * b).log2() / 2f64).log2();
    if smoothed.is_nan() {
        return compact_rgb(0, 0, 0);
    }

    let idx = ((iteration as f64 + 10f64 - smoothed).sqrt() * 256_f64).floor() as i32 % N as i32;
    let (r, g, b) = palette[idx as usize];
    compact_rgb(r, g, b)
}

fn compact_rgb(r: u8, g: u8, b: u8) -> i32 {
    let a = 255;
    ((b as f64 * u8::MAX as f64) as i32) | ((g as f64 * u8::MAX as f64) as i32) << 8 | ((r as f64 * u8::MAX as f64) as i32) << 16 | ((a as f64 * u8::MAX as f64) as i32) << 24
}