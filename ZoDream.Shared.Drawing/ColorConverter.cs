using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ZoDream.Shared.Drawing
{
    public static class ColorConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte From16BitTo8Bit(ushort code) => (byte)(((code * 255) + 32895) >> 16);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort From8BitTo16Bit(byte code) => (ushort)(code * 257);

        public static ushort From16BitToShort(byte a, byte b) => (ushort)((a << 8) | b);

        public static byte FromHalfToByte(byte[] buffer, int startIndex)
        {
            return (byte)Math.Round((float)BitConverter.ToHalf(buffer, startIndex) * 255f);
        }
        public static byte FromFloatToByte(byte[] buffer, int startIndex)
        {
            return (byte)Math.Round(BitConverter.ToSingle(buffer, startIndex) * 255f);
        }

        public static uint RotateRight(uint value, int count)
        {
            return value >> count | value << 32 - count;
        }
        public static int Sum(Vector3 a)
        {
            return (int)(a.X + a.Y + a.Z);
        }

        public static int Sum(Vector4 a)
        {
            return (int)(a.X + a.Y + a.Z + a.W);
        }

        public static byte[] SplitByte(byte[] input, int start, out int length, 
            params int[] chunks)
        {
            var sum = chunks.Sum();
            length = (int)Math.Ceiling((double)sum / 8);
            var total = 0u;
            for (var i = 0; i < length; i++)
            {
                total = (total << 8) + input[start + i];
            }
            var res = new byte[chunks.Length];
            for (var i = 0; i < chunks.Length; i++)
            {
                sum -= chunks[i];
                res[i] = (byte)((total >> sum) & MaxValue(chunks[i]));
            }
            return res;
        }

        private static uint MaxValue(int size)
        {
            var total = 0u;
            for (var i = 0; i < size; i++)
            {
                total = (total << 1) + 1;
            }
            return total;
        }
    }
}
