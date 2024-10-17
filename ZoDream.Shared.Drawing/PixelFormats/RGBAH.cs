using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class RGBAH : SwapBufferDecoder
    {
        public override int ColorSize => 8;
        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = ColorNumerics.FromHalfToByte(input, inputOffset);
            output[outputOffset + 1] = ColorNumerics.FromHalfToByte(input, inputOffset + 2);
            output[outputOffset + 2] = ColorNumerics.FromHalfToByte(input, inputOffset + 4);
            output[outputOffset + 3] = ColorNumerics.FromHalfToByte(input, inputOffset + 6);
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            throw new NotImplementedException();
        }
    }
}
