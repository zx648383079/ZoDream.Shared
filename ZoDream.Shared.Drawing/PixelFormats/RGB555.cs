using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RGB555 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < (size * 2); i += 2)
            {
                var packed = ColorConverter.From16BitToShort(data[i], data[i + 1]);
                buffer[i * 2] = (byte)(packed & 0x7C00);
                buffer[i * 2 + 1] = (byte)(packed & 0x03E0);
                buffer[i * 2 + 2] = (byte)(packed & 0x001F);
                buffer[i * 2 + 3] = byte.MaxValue;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
