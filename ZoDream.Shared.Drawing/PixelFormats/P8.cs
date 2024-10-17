using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class P8 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var buffer = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
            {
                buffer[i * 4 + 0] = data[i];
                buffer[i * 4 + 1] = data[i];
                buffer[i * 4 + 2] = data[i];
                buffer[i * 4 + 3] = 255;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
