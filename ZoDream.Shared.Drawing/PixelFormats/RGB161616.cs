using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class RGB161616 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var offset = i * 6;
                buffer[index] = ColorNumerics.From16BitTo8Bit(
                    ColorNumerics.From16BitToShort(data[offset], data[offset + 1])
                );
                buffer[index + 1] = ColorNumerics.From16BitTo8Bit(
                    ColorNumerics.From16BitToShort(data[offset + 2], data[offset + 3])
                );
                buffer[index + 2] = ColorNumerics.From16BitTo8Bit(
                    ColorNumerics.From16BitToShort(data[offset + 4], data[offset + 5])
                );
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
