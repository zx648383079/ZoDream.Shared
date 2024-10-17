using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public class FileImageData(string fileName) : BaseImageData
    {
        public override SKBitmap? ToBitmap()
        {
            return SKBitmap.Decode(fileName);
        }

        public override SKImage? ToImage()
        {
            return SKImage.FromEncodedData(fileName);
        }
    }
}
