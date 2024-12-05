using System;

namespace ZoDream.Shared.Drawing
{
    public class A16 : IBufferDecoder
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
                buffer[index + 1] = 0xFF;
                buffer[index + 2] = 0xFF;
                buffer[index + 3] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[offset], data[offset + 1]));
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
