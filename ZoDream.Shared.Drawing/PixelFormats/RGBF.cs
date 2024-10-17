using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RGBF : SwapBufferDecoder
    {
        public override int ColorSize => 12;
        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = ColorNumerics.FromFloatToByte(input, inputOffset);
            output[outputOffset + 1] = ColorNumerics.FromFloatToByte(input, inputOffset + 4);
            output[outputOffset + 2] = ColorNumerics.FromFloatToByte(input, inputOffset + 8);
            output[outputOffset + 3] = byte.MaxValue;
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            throw new NotImplementedException();
        }
    }
}
