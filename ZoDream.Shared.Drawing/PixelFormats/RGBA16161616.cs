using System;

namespace ZoDream.Shared.Drawing
{
    public class RGBA16161616 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var offset = i * 6;
                buffer[index] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[offset], data[offset + 1])
                );
                buffer[index + 1] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[offset + 2], data[offset + 3])
                );
                buffer[index + 2] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[offset + 4], data[offset + 5])
                );
                buffer[index + 3] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[offset + 6], data[offset + 7])
                );
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
