using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RGB565 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < (size * 2); i += 2)
            {
                var temp = ColorNumerics.From16BitToShort(data[i], data[i + 1]);
                //var red = (byte)((temp >> 11) & 0x1f);
                //var green = (byte)((temp >> 5) & 0x3f);
                //var blue = (byte)(temp & 0x1f);

                //buffer[(i * 2) + 0] = (byte)((red << 3) | (red >> 2));
                //buffer[(i * 2) + 1] = (byte)((green << 2) | (green >> 4));
                //buffer[(i * 2) + 2] = (byte)((blue << 3) | (blue >> 2));
                //buffer[(i * 2) + 3] = 0xFF;
                buffer[(i * 2) + 0] = (byte)(temp & 0xF800);
                buffer[(i * 2) + 1] = (byte)(temp & 0x07E0);
                buffer[(i * 2) + 2] = (byte)(temp & 0x001F);
                buffer[(i * 2) + 3] = 0xFF;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
