using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZoDream.Shared.Drawing
{
    public class StreamImageData(Stream stream): BaseImageData
    {
        public override SKBitmap? ToBitmap()
        {
            return SKBitmap.Decode(stream);
        }

        public override SKImage? ToImage()
        {
            return SKImage.FromEncodedData(stream);
        }
    }
}
