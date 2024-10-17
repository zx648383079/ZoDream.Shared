using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class VU88(bool swapXY = false) : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (int i = 0; i < (size * 2); i += 2)
            {
                byte X = (byte)(data[i + 1] + 127);
                byte Y = (byte)(data[i + 0] + 127);

                buffer[i * 2] = 0xFF;
                buffer[(i * 2) + 1] = swapXY ? Y : X;
                buffer[(i * 2) + 2] = swapXY ? X : Y;
                buffer[(i * 2) + 3] = 0xFF;
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            var buffer = new byte[height * width * 2];
            for (int i = 0; i < height * width * 2; i += 2)
            {
                int index = 2 * i;
                buffer[i] = data[index + 2]; // V 
                buffer[i + 1] = data[index + 1]; // U
            }
            return buffer;
        }
    }
}
