using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class Y16 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                // 16 bit color, but stored in 8 bits, precision loss, we can use the most important byte and truncate the rest for now.
                // ushort color = (ushort)((data[i * 2]) | (data[i * 2 + 1] << 8));

                var index = i * 4;
                buffer[index] = data[i * 2 + 1];
                buffer[index + 1] = data[i * 2 + 1];
                buffer[index + 2] = data[i * 2 + 1];
                buffer[index + 3] = 0;
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
