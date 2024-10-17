using System;

namespace ZoDream.Shared.Drawing
{
    public class ARGBF : SwapBufferDecoder
    {
        public override int ColorSize => 16;
        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset + 3] = ColorNumerics.FromFloatToByte(input, inputOffset);
            output[outputOffset] = ColorNumerics.FromFloatToByte(input, inputOffset + 4);
            output[outputOffset + 1] = ColorNumerics.FromFloatToByte(input, inputOffset + 8);
            output[outputOffset + 2] = ColorNumerics.FromFloatToByte(input, inputOffset + 12);
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            throw new NotImplementedException();
        }
    }
}
