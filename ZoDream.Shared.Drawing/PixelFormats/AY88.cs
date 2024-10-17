using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class AY88 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < (size * 2); i += 2)
            {
                buffer[i * 2] = data[i];
                buffer[i * 2 + 1] = data[i + 1];
                buffer[i * 2 + 2] = data[i + 1];
                buffer[i * 2 + 3] = data[i + 1];
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            var buffer = new byte[height * width * 2];
            for (int i = 0; i < height * width * 2; i += 2)
            {
                int index = 2 * i;
                buffer[i] = data[index];
                buffer[i + 1] = data[index + 3];
            }
            return buffer;
        }
    }
}
