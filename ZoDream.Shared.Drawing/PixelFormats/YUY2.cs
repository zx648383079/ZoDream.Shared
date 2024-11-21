using System;

namespace ZoDream.Shared.Drawing
{
    public class YUY2 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var p = 0;
            var o = 0;
            var halfWidth = width / 2;
            var buffer = new byte[width * height * 4];
            for (var j = 0; j < height; j++)
            {
                for (var i = 0; i < halfWidth; ++i)
                {
                    var y0 = data[p++];
                    var u0 = data[p++];
                    var y1 = data[p++];
                    var v0 = data[p++];
                    var c = y0 - 16;
                    var d = u0 - 128;
                    var e = v0 - 128;
                    buffer[o++] = (byte)Math.Clamp((298 * c + 516 * d + 128) >> 8, byte.MinValue, byte.MaxValue);            // b
                    buffer[o++] = (byte)Math.Clamp((298 * c - 100 * d - 208 * e + 128) >> 8, byte.MinValue, byte.MaxValue);  // g
                    buffer[o++] = (byte)Math.Clamp((298 * c + 409 * e + 128) >> 8, byte.MinValue, byte.MaxValue);            // r
                    buffer[o++] = byte.MaxValue;
                    c = y1 - 16;
                    buffer[o++] = (byte)Math.Clamp((298 * c + 516 * d + 128) >> 8, byte.MinValue, byte.MaxValue);            // b
                    buffer[o++] = (byte)Math.Clamp((298 * c - 100 * d - 208 * e + 128) >> 8, byte.MinValue, byte.MaxValue);  // g
                    buffer[o++] = (byte)Math.Clamp((298 * c + 409 * e + 128) >> 8, byte.MinValue, byte.MaxValue);            // r
                    buffer[o++] = byte.MaxValue;
                }
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            var buffer = new byte[width * height * 2];
            var ptrY = 0;
            for (var i = 0; i < height; i++)
            {
                var begin = i * width;
                for (var j = 0; j < width; j++)
                {
                    var offset = (begin + j) * 4;
                    var r = data[offset];
                    var g = data[offset + 1];
                    var b = data[offset + 2];
                    var y = ((66 * r + 129 * g + 25 * b) >> 8) + 16;
                    var u = ((-38 * r - 74 * g + 112 * b) >> 8) + 128;
                    var v = ((112 * r - 94 * g - 18 * b) >> 8) + 128;
                    if (j % 2 == 0)
                    {
                        buffer[ptrY++] = (byte)y;
                        buffer[ptrY++] = (byte)u;
                    } else
                    {
                        buffer[ptrY++] = (byte)y;
                        buffer[ptrY++] = (byte)v;
                    }
                }
            }
            return buffer;
        }
    }
}
