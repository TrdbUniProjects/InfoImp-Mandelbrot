// When using neither the rayon nor the ocl feature,
// rustc will complain of unused functions
#![cfg_attr(not(any(feature = "rayon", feature = "ocl")), allow(unused))]

#[cfg(feature = "rayon")]
mod rayon;
#[cfg(feature = "ocl")]
mod ocl;

fn get_color(iteration: i32, a: f64, b: f64, color_palette: i32) -> i32 {
    match color_palette {
        0 => get_color_with_palette(iteration, a, b, get_default_color_palette()),
        1 => get_color_with_palette(iteration, a, b, get_redscale_color_palette()),
        _ => panic!("Invalid color palette {color_palette}")
    }
}

fn get_color_palette_len(palette: i32) -> usize {
    match palette {
        0 => get_default_color_palette().len(),
        1 => get_redscale_color_palette().len(),
        _ => panic!("Invalid color palette {palette}"),
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