using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class LA1616 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var l = ColorConverter.From16BitTo8Bit(ColorConverter.From16BitToShort(data[index], data[index + 1]));
                buffer[index] = l;
                buffer[index + 1] = l;
                buffer[index + 2] = l;
                buffer[index + 3] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(data[index + 2], data[index + 3])); ;
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
