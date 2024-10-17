using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public class BitmapImageData(SKBitmap bitmap): BaseImageData
    {
        public override SKBitmap? ToBitmap()
        {
            return bitmap.Copy();
        }

        public override SKImage? ToImage()
        {
            return SKImage.FromBitmap(bitmap);
        }
    }
}
