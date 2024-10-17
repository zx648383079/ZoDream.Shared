using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class A8 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                int index = i * 4;
                buffer[index] = 0xFF;
                buffer[index + 1] = 0xFF;
                buffer[index + 2] = 0xFF;
                buffer[index + 3] = data[i];
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            var buffer = new byte[height * width];
            for (var i = 0; i < (buffer.Length); i++)
            {
                var index = i * 4;
                buffer[i] = data[index];
            }
            return buffer;
        }
    }
}
