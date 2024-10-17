using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class VU1616(bool swapXY = false) : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (int i = 0; i < buffer.Length; i += 4)
            {
                var X = (ushort)(((((ushort)data[i + 2]) << 8) | (ushort)data[i + 3]) + 0x7FFF);
                var Y = (ushort)(((((ushort)data[i + 0]) << 8) | (ushort)data[i + 1]) + 0x7FFF);

                if (swapXY)
                {
                    buffer[i] = (byte)((X >> 8) & 0xFF);
                    buffer[(i) + 1] = (byte)(X & 0xFF);
                    buffer[(i) + 2] = (byte)((Y >> 8) & 0xFF);
                    buffer[(i) + 3] = (byte)(Y & 0xFF);
                }
                else
                {
                    buffer[i] = (byte)((Y >> 8) & 0xFF);
                    buffer[(i) + 1] = (byte)(Y & 0xFF);
                    buffer[(i) + 2] = (byte)((X >> 8) & 0xFF);
                    buffer[(i) + 3] = (byte)(X & 0xFF);
                }
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
