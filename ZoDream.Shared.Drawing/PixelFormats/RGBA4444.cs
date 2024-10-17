using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RGBA4444 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (int i = 0; i < (size * 2); i += 2)
            {
                buffer[i * 2 + 2] = (byte)((data[i + 1] & 0x0F) << 4);
                buffer[i * 2 + 3] = (byte)(data[i + 1] & 0xF0);
                buffer[i * 2 + 0] = (byte)((data[i] & 0x0F) << 4);
                buffer[i * 2 + 1] = (byte)(data[i] & 0xF0);
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
