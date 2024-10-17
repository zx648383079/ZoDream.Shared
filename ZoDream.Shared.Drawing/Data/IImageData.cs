using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public interface IImageData
    {
        public SKBitmap? ToBitmap();
        public SKImage? ToImage();
    }
}
