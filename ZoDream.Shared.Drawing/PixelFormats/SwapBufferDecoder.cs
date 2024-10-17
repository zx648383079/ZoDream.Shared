using System;
using System.Linq;

namespace ZoDream.Shared.Drawing
{
    public abstract class SwapBufferDecoder : IBufferDecoder
    {
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

        protected abstract void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset);

        public byte[] Encode(byte[] data, int width, int height)
        {
            var colorSize = ColorSize;
            var size = width * height;
            var buffer = new byte[size * colorSize];
            for (var i = 0; i < size; i++)
            {
                Decode(data, i * 4, buffer, i * colorSize);
            }
            return buffer;
        }

        protected abstract void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset);
    }
    public class RGBASwapDecoder(byte[] maps) : SwapBufferDecoder
    {
        public const string RGBA = "RGBA";
        public const string ARGB = "ARGB";
        public const string BGRA = "BGRA";
        public const string ABGR = "ABGR";

        public RGBASwapDecoder(string name) : this(ConvertMap(name))
        {
            
        }

        private static byte[] ConvertMap(string name)
        {
            return ConvertMap(name.ToCharArray());
        }

        private static byte[] ConvertMap(params char[] maps)
        {
            return maps.Select(code => {
                return code switch
                {
                    'R' or 'r' => (byte)0,
                    'G' or 'g' => (byte)1,
                    'B' or 'b' => (byte)2,
                    'A' or 'a' => (byte)3,
                    _ => (byte)9
                };
            }).Where(i => i < 0).Take(4).ToArray();
        }

        protected override void Decode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < 4; i++)
            {
                output[outputOffset + i] = input[inputOffset + maps[i]];
            }
        }

        protected override void Encode(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            for (var i = 0; i < 4; i++)
            {
                output[outputOffset + maps[i]] = input[inputOffset + i];
            }
        }
    }
}
