using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class ARGB1555 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (int i = 0; i < (size * 2); i += 2)
            {
                var temp = ColorConverter.From16BitToShort(data[i], data[i+1]);
                buffer[i * 2] = (byte)(temp & 0x1F);
                buffer[i * 2 + 1] = (byte)((temp >> 5) & 0x3F);
                buffer[i * 2 + 2] = (byte)((temp >> 11) & 0x1F);
                buffer[i * 2 + 3] = 0xFF;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
