using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    // R half
    public class RH : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i ++)
            {
                var index = i * 4;
                buffer[index] = ColorNumerics.FromHalfToByte(data, index / 2);
                buffer[index + 1] = 0;
                buffer[index + 2] = 0;
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
