using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class L16 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var l = ColorConverter.From16BitToShort(data[i * 2], data[i * 2 + 1]);
                var b = ColorConverter.From16BitTo8Bit(l);
                var index = i * 4;
                buffer[index] = b;
                buffer[index + 1] = b;
                buffer[index + 2] = b;
                buffer[index + 3] = byte.MaxValue;
            }

            return buffer;
            
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
