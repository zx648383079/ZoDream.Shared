using SkiaSharp;
using System.IO;

namespace ZoDream.Shared.Drawing
{
    public class SKBitmapDecoder : IBitmapDecoder
    {
        public static bool IsSupport(BitmapFormat format)
        {
            return Parse(format) != SKColorType.Unknown;
        }

        public static SKColorType Parse(BitmapFormat format)
        {
            return format switch
            {
                BitmapFormat.A8 => SKColorType.Alpha8,
                BitmapFormat.A16 => SKColorType.Alpha16,
                BitmapFormat.G8 => SKColorType.Gray8,
                BitmapFormat.RG1616 => SKColorType.Rg1616,
                BitmapFormat.RGBX8888 => SKColorType.Rgb888x,
                BitmapFormat.RGB565 => SKColorType.Rgb565,
                BitmapFormat.RGBA1010102 => SKColorType.Rgba1010102,
                BitmapFormat.RGBA16161616 => SKColorType.Rgba16161616,
                BitmapFormat.BGRA8888 => SKColorType.Bgra8888,
                BitmapFormat.RGBA4444 => SKColorType.Argb4444,
                BitmapFormat.RGBA8888 => SKColorType.Rgba8888,
                BitmapFormat.RG88 => SKColorType.Rg88,
                _ => SKColorType.Unknown,
            };
        }

        public static IImageData Decode(byte[] data, int width, int height, BitmapFormat format)
        {
            return Decode(data, width, height, Parse(format));
        }
        /// <summary>
        /// 把像素字节数组转图片
        /// </summary>
        /// <param name="buffer">像素数组，例如: [r, g, b, a, r, g, b, a ...]</param>
        /// <param name="width">图片的宽度，例如: 512</param>
        /// <param name="height">图片的高度，例如: 1024</param>
        /// <param name="format">指定像素数组的组成方式，例如：SKColorType.Rgba8888</param>
        /// <returns></returns>
        public static IImageData Decode(byte[] buffer, int width, int height, SKColorType format)
        {
            return new ByteImageData(buffer, width, height, format);
        }

        public IImageData Decode(byte[] data)
        {
            return new FileByteImageData(data);
        }

        public IImageData Decode(Stream input)
        {
            return new StreamImageData(input);
        }

        public IImageData Decode(string fileName)
        {
            return new FileImageData(fileName);
        }

        public byte[] Encode(IImageData data)
        {
            var res = data.ToImage()?.Encode(SKEncodedImageFormat.Png, 100);
            if (res is null)
            {
                return [];
            }
            return res.AsSpan().ToArray();
        }

        public void Encode(IImageData data, Stream output)
        {
            data.ToImage()?.Encode(output, SKEncodedImageFormat.Png, 100);
        }

        public void Encode(IImageData data, string fileName)
        {
            data.ToImage()?.SaveAs(fileName);
        }
    }
}
