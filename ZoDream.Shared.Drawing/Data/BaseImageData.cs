using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public abstract class BaseImageData: IImageData
    {

        public abstract SKBitmap? ToBitmap();
        public abstract SKImage? ToImage();
    }
}
