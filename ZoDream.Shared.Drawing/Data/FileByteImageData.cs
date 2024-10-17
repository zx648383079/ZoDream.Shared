using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public class FileByteImageData(byte[] buffer) : BaseImageData
    {
        public override SKBitmap? ToBitmap()
        {
            return SKBitmap.Decode(buffer);
        }

        public override SKImage? ToImage()
        {
            return SKImage.FromEncodedData(buffer);
        }
    }
}
