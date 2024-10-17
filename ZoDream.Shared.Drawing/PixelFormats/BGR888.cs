namespace ZoDream.Shared.Drawing
{
    public class BGR888 : SwapBufferDecoder
    {
        public override int ColorSize => 3;

        private readonly byte[] _maps = [2, 1, 0];

        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < _maps.Length; i++)
            {
                output[outputOffset + i] = input[inputOffset + _maps[i]];
            }
            output[outputOffset + 3] = byte.MaxValue;
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < _maps.Length; i++)
            {
                output[outputOffset + _maps[i]] = input[inputOffset + i];
            }
        }
    }
}
