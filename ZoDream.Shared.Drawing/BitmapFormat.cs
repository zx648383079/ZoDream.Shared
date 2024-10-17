namespace ZoDream.Shared.Drawing
{
    public enum BitmapFormat : sbyte
    {
        Unknown,
        A8,                 // 0x00, alpha
        A16,
        Y8,                 // 0x01, intensity
        AY8,                // 0x02, combined alpha-intensity
        AY88,               // 0x03, separate alpha-intensity
        Unused4,            // 0x04
        Unused5,            // 0x05
        RGB565,             // 0x06, high-color
        RGB655,             // 0x07 r6g5b5
        RGBA5551,           // 0x08, high-color with 1-bit alpha
        RGBX8888,           // 0x0A, true-color
        RGBA8888,           // 0x0B, true-color with alpha
        UnusedC,            // 0x0C
        UnusedD,            // 0x0D
        DXT1,               // 0x0E, compressed with color-key transparency ('DXT1')
        DXT2,
        DXT3,               // 0x0F, compressed with explicit alpha ('DXT3')
        DXT4,
        DXT5,               // 0x10, compressed with interpolated alpha ('DXT5')
        A4R4G4B4Font,       // 0x11, font format
        P8,                 // 0x12 palettized
        ARGBFP32,           // 0x13 SOFTWARE 32 bit float ARGB
        RGBFP32,            // 0x14 SOFTWARE 32 bit float RGB
        RGBFP16,            // 0x15 SOFTWARE 16 bit float RGB
        VU88,               // 0x16, v8u8 signed 8-bit normals
        G8,               // 0x17 g8b8 unsigned 8-bit
        GB88,               // 0x17 g8b8 unsigned 8-bit
        A32B32G32R32F,      // 0x18, 32 bit float ABGR
        A16B16G16R16F,      // 0x19, 16 bit float ABGR
        Q8W8V8U8,           // 0x1A, 8 bit signed 4 channel
        VU1616,             // 0x1D, v16u16 signed 16-bit normals
        Unused1E,           // 0x1E compressed 4-bit single channel
        Dxt5a,              // 0x1F compressed interpolated single channel
        Y16,                // 0x20 compressed channel mask, Reach: 16 bits monochrome texture
        Dxn,                // 0x21, compressed normals: high quality ('ATI2')
        CTX1,               // 0x22  compressed normals: high compression
        Dxt3aAlpha,         // 0x23 compressed 4-bit alpha channel
        Dxt3aMono,          // 0x24 compressed interpolated alpha channel
        Dxt5aAlpha,         // 0x25 compressed 4-bit monochrome
        Dxt5aMono,          // 0x26, Reach DXN compressed interpolated monochrome
        DxnMonoAlpha,       // 0x27, Reach CTX1 compressed interpolated monochrome + alpha
        ReachDxt3aMono,     // 0x28, Reach Dxt3aMono
        ReachDxt3aAlpha,    // 0x29, Reach Dxt3aAlpha
        ReachDxt5aMono,     // 0x2A, Reach Dxt5aAlpha
        ReachDxt5aAlpha,    // 0x2B, Reach Dxt5aMono
        ReachDxnMonoAlpha,   // 0x2C, Reach DxnMonoAlpha

        PVRTC_RGB2,
        PVRTC_RGBA2,
        PVRTC_RGB4,
        PVRTC_RGBA4,

        ETC_RGB8,

        RG88,
        RG1616,
        RGBA4444,
        RGB555,
        RGB888,
        RGBA1010102,
        RGBA16161616,

        BGRA8888,
        ARGB8888,
        BGR888,
        RGB161616,
        R8,
        R16,
        RH, // R half 16b
        RGH, // RG H 16 16
        RGBAH,
        RF, // R float 32b
        RGF,
        RGBF,
        ARGBF,
        RGB9e5F,
        L8,
        LA88,
        L16,
        LA1616,


    }
}
