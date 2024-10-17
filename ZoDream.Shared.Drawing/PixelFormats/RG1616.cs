using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RG1616 : IBufferDecoder
    {
        private static readonly Vector2 Max = new(ushort.MaxValue);
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            for (var i = 0; i < size; i++)
            {
                var index = i * 4;
                var packed = Pack(new(ColorNumerics.From16BitToShort(data[index], data[index + 1]), ColorNumerics.From16BitToShort(data[index + 2], data[index + 3])));
                buffer[index] = ColorNumerics.From16BitTo8Bit((ushort)(packed & 0xFFFF));
                buffer[index + 1] = ColorNumerics.From16BitTo8Bit((ushort)(packed >> 16));
                buffer[index + 2] = byte.MinValue;
                buffer[index + 3] = byte.MaxValue;
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
        private static uint Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, Vector2.Zero, Vector2.One) * Max;
            return (uint)(((int)Math.Round(vector.X) & 0xFFFF) | (((int)Math.Round(vector.Y) & 0xFFFF) << 16));
        }
    }
}
