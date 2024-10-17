using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class RGBA1010102 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var packed = BitConverter.ToUInt32(data, index);
                buffer[index] = (byte)(((packed >> 0) & 0x03FF) / 1023F);
                buffer[index + 1] = (byte)(((packed >> 10) & 0x03FF) / 1023F);
                buffer[index + 2] = (byte)(((packed >> 20) & 0x03FF) / 1023F);
                buffer[index + 3] = (byte)(((packed >> 30) & 0x03) / 3);
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
