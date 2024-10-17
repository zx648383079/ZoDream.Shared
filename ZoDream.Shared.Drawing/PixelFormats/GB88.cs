using System;

namespace ZoDream.Shared.Drawing
{
    public class GB88 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var offset = i * 2;
                buffer[index] = 0xFF;
                buffer[index + 1] = data[offset + 1];
                buffer[index + 2] = data[offset];
                buffer[index + 3] = byte.MaxValue;
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
