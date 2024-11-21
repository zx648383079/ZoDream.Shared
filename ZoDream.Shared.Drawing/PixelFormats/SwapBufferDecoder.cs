using System;
using System.Linq;

namespace ZoDream.Shared.Drawing
{
    public abstract class SwapBufferDecoder : IBufferDecoder
    {
        /// <summary>
        /// 一个颜色值占的字节数
        /// </summary>
        public virtual int ColorSize => 4;

        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            var colorSize = ColorSize;
            for (var i = 0; i < size; i++)
            {
                Decode(data, i * colorSize, buffer, i * 4);
            }
            return buffer;
        }
        /// <summary>
        /// 解码一个颜色
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputOffset"></param>
        /// <param name="output"></param>
        /// <param name="outputOffset"></param>
        protected abstract void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset);

        public byte[] Encode(byte[] data, int width, int height)
        {
            var colorSize = ColorSize;
            var size = width * height;
            var buffer = new byte[size * colorSize];
            for (var i = 0; i < size; i++)
            {
                Encode(data, i * 4, buffer, i * colorSize);
            }
            return buffer;
        }
        /// <summary>
        /// 编码一个颜色
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputOffset"></param>
        /// <param name="output"></param>
        /// <param name="outputOffset"></param>
        protected abstract void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset);
    }
    public class RGBASwapDecoder(byte[] maps) : SwapBufferDecoder
    {
        public const string RGBA = "RGBA";
        public const string ARGB = "ARGB";
        public const string BGRA = "BGRA";
        public const string ABGR = "ABGR";
        const byte R = 0;
        const byte G = 1;
        const byte B = 2;
        const byte A = 3;
        const byte X = 9;



        public RGBASwapDecoder(string name) : this(ConvertMap(name))
        {

        }
        /// <summary>
        /// 一个颜色空间占的字节数
        /// </summary>
        public virtual int ColorSpaceSize => 1;
        public override int ColorSize => maps.Length * ColorSpaceSize;

        internal static byte[] ConvertMap(string name)
        {
            return ConvertMap(name.ToCharArray());
        }

        private static byte[] ConvertMap(params char[] maps)
        {
            return maps.Select(code => {
                return code switch
                {
                    'R' or 'r' => R,
                    'G' or 'g' => G,
                    'B' or 'b' => B,
                    'A' or 'a' => A,
                    _ => X
                };
            }).Where(i => i < 0).Take(4).ToArray();
        }

        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < maps.Length; i++)
            {
                if (maps[i] == X)
                {
                    continue;
                }
                Decode(i, input, inputOffset + i * ColorSpaceSize, output, outputOffset + maps[i]);
            }
            if (!maps.Contains(A))
            {
                output[outputOffset + A] = byte.MaxValue;
            }
        }
        /// <summary>
        /// 解码一个颜色空间
        /// </summary>
        /// <param name="index"></param>
        /// <param name="input"></param>
        /// <param name="inputOffset"></param>
        /// <param name="output"></param>
        /// <param name="outputOffset"></param>
        protected virtual void Decode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = input[inputOffset];
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < maps.Length; i++)
            {
                if (maps[i] == X)
                {
                    continue;
                }
                Encode(i, input, inputOffset + maps[i], output, outputOffset + i * ColorSpaceSize);
            }
        }
        /// <summary>
        /// 编码一个颜色空间
        /// </summary>
        /// <param name="index"></param>
        /// <param name="input"></param>
        /// <param name="inputOffset"></param>
        /// <param name="output"></param>
        /// <param name="outputOffset"></param>
        protected virtual void Encode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = input[inputOffset];
        }
    }

    /// <summary>
    /// 半个字节
    /// </summary>
    /// <param name="maps"></param>
    public class NibbleSwapDecoder(byte[] maps) : SwapBufferDecoder
    {
        public NibbleSwapDecoder(string name) : this(RGBASwapDecoder.ConvertMap(name))
        {

        }
        public override int ColorSize => maps.Length / 2;

        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < maps.Length; i += 2)
            {
                var val = input[inputOffset + i / 2];
                output[outputOffset + maps[i]] = (byte)((val & 0x0F) << 4);
                output[outputOffset + maps[i + 1]] = (byte)(val & 0xF0);
            }
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < maps.Length; i += 2)
            {
                output[outputOffset + i / 2] = (byte)((input[inputOffset + maps[i]] << 4) & 0x0F + input[inputOffset + maps[i + 1]] & 0xf0);
            }
        }
    }

    public class FloatSwapDecoder(string name) : RGBASwapDecoder(name)
    {
        public override int ColorSpaceSize => 4;

        protected override void Decode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = ColorConverter.FromFloatToByte(input, inputOffset);
        }

        protected override void Encode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            var buffer = BitConverter.GetBytes(input[inputOffset] / 255f);
            Array.Copy(buffer, 0, output, outputOffset, 4);
        }
    }

    public class ShortSwapDecoder(string name) : RGBASwapDecoder(name)
    {
        public override int ColorSpaceSize => 2;

        protected override void Decode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = ColorConverter.From16BitTo8Bit(
                    ColorConverter.From16BitToShort(input[inputOffset], input[inputOffset + 1]));
        }

        protected override void Encode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            var buffer = BitConverter.GetBytes(ColorConverter.From8BitTo16Bit(input[inputOffset]));
            Array.Copy(buffer, 0, output, outputOffset, 2);
        }
    }

    public class HalfSwapDecoder(string name) : RGBASwapDecoder(name)
    {
        public override int ColorSpaceSize => 2;

        protected override void Decode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            output[outputOffset] = ColorConverter.FromHalfToByte(input, inputOffset);
        }

        protected override void Encode(int index, byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            var buffer = BitConverter.GetBytes((Half)input[inputOffset]);
            Array.Copy(buffer, 0, output, outputOffset, 2);
        }
    }
}
