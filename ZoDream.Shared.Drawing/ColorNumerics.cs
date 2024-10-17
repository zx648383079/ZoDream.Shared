using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ZoDream.Shared.Drawing
{
    public static class ColorNumerics
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
    }
}
