using System;

namespace ZoDream.Shared.Drawing
{
    public class RF : SwapBufferDecoder
    {
        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = (byte)Math.Round(BitConverter.ToSingle(input, inputOffset) * 255f);
            output[outputOffset + 3] = byte.MaxValue;
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            throw new NotImplementedException();
        }
    }
}
