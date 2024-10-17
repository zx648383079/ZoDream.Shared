using SkiaSharp;

namespace ZoDream.Shared.Drawing
{
    public class ByteImageData(byte[] buffer, int width, int height, SKColorType format) : BaseImageData
    {
        public override SKBitmap? ToBitmap()
        {
            var newInfo = new SKImageInfo(width, height, format);
            var data = SKData.CreateCopy(buffer);
            //return SKBitmap.Decode(data, newInfo);
            //using MemoryStream ms = new(_buffer);
            //using SKManagedStream skStream = new(ms, false);
            //var data = SKData.Create(skStream);
            //var data = SKData.CreateCopy(_buffer);
            //var gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var bitmap = new SKBitmap();
            bitmap.InstallPixels(newInfo, data.Data);
            //bitmap.InstallPixels(newInfo, gcHandle.AddrOfPinnedObject(), newInfo.RowBytes, delegate { 
            //    gcHandle.Free();
            //}, null);
            return bitmap;
        }

        public override SKImage? ToImage()
        {
            var newInfo = new SKImageInfo(width, height, format);
            var data = SKData.CreateCopy(buffer);
            return SKImage.FromPixels(newInfo, data);
        }
    }
}
