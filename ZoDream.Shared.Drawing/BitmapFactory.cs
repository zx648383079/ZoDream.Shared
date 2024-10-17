using SkiaSharp;
using System;

namespace ZoDream.Shared.Drawing
{
    public static class BitmapFactory
    {

        public static IImageData Decode(string fileName)
        {
            return new SKBitmapDecoder().Decode(fileName);
        }

        public static IImageData Decode(byte[] data, int width, int height, BitmapFormat format)
        {
            if (SKBitmapDecoder.IsSupport(format))
            {
                return SKBitmapDecoder.Decode(data, width, height, format);
            }
            IBufferDecoder decoder = format switch
            {
                BitmapFormat.A8 => new A8(),
                BitmapFormat.A16 => new A16(),
                BitmapFormat.Y8 => new Y8(),
                BitmapFormat.AY8 => new AY8(),
                BitmapFormat.AY88 => new AY88(),
                BitmapFormat.RGB565 => new RGB565(),
                BitmapFormat.RGB655 => throw new NotImplementedException(),
                BitmapFormat.RGBA5551 => new ARGB1555(),
                BitmapFormat.RGBX8888 => throw new NotImplementedException(),
                BitmapFormat.DXT1 => new DXT1(),
                BitmapFormat.DXT3 => new DXT3(),
                BitmapFormat.DXT5 => new DXT5(),
                BitmapFormat.P8 => new P8(),
                BitmapFormat.ARGBFP32 => throw new NotImplementedException(),
                BitmapFormat.RGBFP32 => throw new NotImplementedException(),
                BitmapFormat.RGBFP16 => throw new NotImplementedException(),
                BitmapFormat.VU88 => new VU88(),
                BitmapFormat.G8 => new G8(),
                BitmapFormat.GB88 => new GB88(),
                BitmapFormat.VU1616 => new VU1616(),
                BitmapFormat.Y16 => new Y16(),
                BitmapFormat.Dxn => new DXN(),
                BitmapFormat.CTX1 => new CTX1(),
                BitmapFormat.PVRTC_RGB2 => throw new NotImplementedException(),
                BitmapFormat.PVRTC_RGBA2 => throw new NotImplementedException(),
                BitmapFormat.PVRTC_RGB4 => new PVRTC_RGB4(),
                BitmapFormat.PVRTC_RGBA4 => new PVRTC_RGBA4(),
                BitmapFormat.ETC_RGB8 => throw new NotImplementedException(),
                BitmapFormat.RG88 => new RG88(),
                BitmapFormat.RG1616 => new RG1616(),
                BitmapFormat.RGBA4444 => new RGBA4444(),
                BitmapFormat.RGB555 => new RGB555(),
                BitmapFormat.RGB888 => new RGB888(),
                BitmapFormat.RGBA1010102 => new RGBA1010102(),
                BitmapFormat.RGBA16161616 => new RGBA16161616(),
                BitmapFormat.BGRA8888 => new BGRA8888(),
                BitmapFormat.ARGB8888 => new RGBASwapDecoder("ARGB"),
                BitmapFormat.BGR888 => new BGR888(),
                BitmapFormat.RGB161616 => new RGB161616(),
                BitmapFormat.RH => new RH(),
                BitmapFormat.RGH => new RGH(),
                BitmapFormat.RGBAH => new RGBAH(),
                BitmapFormat.RF => new RF(),
                BitmapFormat.RGF => new RGF(),
                BitmapFormat.RGBF => new RGBF(),
                BitmapFormat.ARGBF => new ARGBF(),
                BitmapFormat.RGB9e5F => new RGB9e5F(),
                BitmapFormat.R16 => new R16(),
                BitmapFormat.L8 => new L8(),
                BitmapFormat.LA88 => new LA88(),
                BitmapFormat.L16 => new L16(),
                BitmapFormat.LA1616 => new LA1616(),
                _ => throw new NotImplementedException(),
            };
            return SKBitmapDecoder.Decode(decoder.Decode(data, width, height), width, height, SKColorType.Rgba8888);
        }
    }
}
