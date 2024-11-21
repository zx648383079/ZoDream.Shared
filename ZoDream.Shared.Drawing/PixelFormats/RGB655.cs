using System;

namespace ZoDream.Shared.Drawing
{
    internal class RGB655 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < (size * 2); i += 2)
            {
                var j = i * 2;
                var res = ColorConverter.SplitByte(data, i, out _, 6, 5, 5);
                buffer[j] = res[0];
                buffer[j + 1] = res[1];
                buffer[j + 2] = res[2];
                buffer[j + 3] = byte.MaxValue;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
