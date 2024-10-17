using System;
using System.Numerics;
using ZoDream.Shared.Drawing.PVRTC;

namespace ZoDream.Shared.Drawing
{
    public class PVRTC_RGBA4 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = Math.Max(width, height);
            var blocks = size / 4;
            var blockMask = blocks - 1;

            var buffer = new byte[size * size * 4];

            var packets = new Packet[blocks * blocks];
            var eightBytes = new byte[8];
            for (var i = 0; i < packets.Length; i++)
            {
                packets[i] = new Packet();
                Buffer.BlockCopy(data, i * 8, eightBytes, 0, 8);
                packets[i].InitFromBytes(eightBytes);
            }

            var currentFactorIndex = 0;

            for (var y = 0; y < blocks; ++y)
            {
                for (var x = 0; x < blocks; ++x)
                {
                    currentFactorIndex = 0;

                    var packet = packets[MortonTable.GetMortonNumber(x, y)];

                    uint mod = packet.GetModulationData();

                    for (int py = 0; py < 4; ++py)
                    {
                        var yOffset = py < 2 ? -1 : 0;
                        var y0 = y + yOffset & blockMask;
                        var y1 = y0 + 1 & blockMask;

                        for (int px = 0; px < 4; ++px)
                        {
                            int xOffset = px < 2 ? -1 : 0;
                            int x0 = x + xOffset & blockMask;
                            int x1 = x0 + 1 & blockMask;

                            var p0 = packets[MortonTable.GetMortonNumber(x0, y0)];
                            var p1 = packets[MortonTable.GetMortonNumber(x1, y0)];
                            var p2 = packets[MortonTable.GetMortonNumber(x0, y1)];
                            var p3 = packets[MortonTable.GetMortonNumber(x1, y1)];

                            var currentFactors = Packet.BILINEAR_FACTORS[currentFactorIndex];

                            var ca = p0.GetColorRgbaA() * currentFactors[0] +
                                            p1.GetColorRgbaA() * currentFactors[1] +
                                            p2.GetColorRgbaA() * currentFactors[2] +
                                            p3.GetColorRgbaA() * currentFactors[3];

                            var cb = p0.GetColorRgbaB() * currentFactors[0] +
                                            p1.GetColorRgbaB() * currentFactors[1] +
                                            p2.GetColorRgbaB() * currentFactors[2] +
                                            p3.GetColorRgbaB() * currentFactors[3];

                            var currentWeights = Packet.WEIGHTS[4 * packet.GetPunchThroughAlpha() + mod & 3];

                            var red = (byte)((int)(ca.X * currentWeights[0] + cb.X * currentWeights[1]) >> 7);
                            var green = (byte)((int)(ca.Y * currentWeights[0] + cb.Y * currentWeights[1]) >> 7);
                            var blue = (byte)((int)(ca.Z * currentWeights[0] + cb.Z * currentWeights[1]) >> 7);
                            var alpha = (byte)((int)(ca.W * currentWeights[2] + cb.W * currentWeights[3]) >> 7);
                            var pX = px + x * 4;
                            var pY = py + y * 4;
                            var offset = pX + pY * size;
                            buffer[offset] = red;
                            buffer[offset + 1] = green;
                            buffer[offset + 2] = green;
                            buffer[offset + 3] = alpha;
                            mod >>= 2;
                            currentFactorIndex++;
                        }
                    }
                }
            }

            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            var size = Math.Max(width, height);

            if (!((size & size - 1) == 0))
            {
                throw new ArgumentException("Texture resolution must be 2^N!");
            }

            var blocks = size / 4;
            var blockMask = blocks - 1;

            var packets = new Packet[blocks * blocks];
            for (var i = 0; i < packets.Length; i++)
            {
                packets[i] = new Packet();
            }

            for (int y = 0; y < blocks; ++y)
            {
                for (int x = 0; x < blocks; ++x)
                {
                    (byte[] minColor, byte[] maxColor) = GetMinMaxColorsWithAlpha(data, 4 * x, 4 * y);

                    Packet packet = packets[MortonTable.GetMortonNumber(x, y)];
                    packet.SetPunchThroughAlpha(false);
                    packet.SetColorA(minColor[0], minColor[1], minColor[2], minColor[3]);
                    packet.SetColorB(maxColor[0], maxColor[1], maxColor[2], maxColor[3]);
                }
            }

            int currentFactorIndex = 0;
            var color = new byte[4];

            for (int y = 0; y < blocks; ++y)
            {
                for (int x = 0; x < blocks; ++x)
                {
                    currentFactorIndex = 0;

                    uint modulationData = 0;

                    for (int py = 0; py < 4; ++py)
                    {
                        int yOffset = py < 2 ? -1 : 0;
                        int y0 = y + yOffset & blockMask;
                        int y1 = y0 + 1 & blockMask;

                        for (int px = 0; px < 4; ++px)
                        {
                            int xOffset = px < 2 ? -1 : 0;
                            int x0 = x + xOffset & blockMask;
                            int x1 = x0 + 1 & blockMask;

                            Packet p0 = packets[MortonTable.GetMortonNumber(x0, y0)];
                            Packet p1 = packets[MortonTable.GetMortonNumber(x1, y0)];
                            Packet p2 = packets[MortonTable.GetMortonNumber(x0, y1)];
                            Packet p3 = packets[MortonTable.GetMortonNumber(x1, y1)];

                            var currentFactors = Packet.BILINEAR_FACTORS[currentFactorIndex];

                            var ca = p0.GetColorRgbaA() * currentFactors[0] +
                                                p1.GetColorRgbaA() * currentFactors[1] +
                                                p2.GetColorRgbaA() * currentFactors[2] +
                                                p3.GetColorRgbaA() * currentFactors[3];

                            var cb = p0.GetColorRgbaB() * currentFactors[0] +
                                                p1.GetColorRgbaB() * currentFactors[1] +
                                                p2.GetColorRgbaB() * currentFactors[2] +
                                                p3.GetColorRgbaB() * currentFactors[3];

                            Buffer.BlockCopy(data, (4 * x + px) * (4 * y + py), color, 0, 4);
                            var d = cb - ca;
                            var p = new Vector4(color[0] * 16, color[1] * 16, 
                                color[2] * 16, color[3] * 16);
                            var v = p - ca;

                            // PVRTC uses weightings of 0, 3/8, 5/8 and 1
                            // The boundaries for these are 3/16, 1/2 (=8/16), 13/16
                            int projection = ColorNumerics.Sum(v * d) * 16; //Mathf.RoundToInt(Vector4.Dot(v, d)) * 16;
                            int lengthSquared = ColorNumerics.Sum(d * d); //Mathf.RoundToInt(Vector4.Dot(d,d));
                            if (projection > 3 * lengthSquared) modulationData++;
                            if (projection > 8 * lengthSquared) modulationData++;
                            if (projection > 13 * lengthSquared) modulationData++;

                            modulationData = ColorNumerics.RotateRight(modulationData, 2);

                            currentFactorIndex++;
                        }
                    }

                    Packet packet = packets[MortonTable.GetMortonNumber(x, y)];
                    packet.SetModulationData(modulationData);
                }
            }

            var returnValue = new byte[size * size / 2];

            // Create final byte array from PVRTC packets
            for (int i = 0; i < packets.Length; i++)
            {
                var tempArray = packets[i].GetAsByteArray();
                Buffer.BlockCopy(tempArray, 0, returnValue, 8 * i, 8);
            }

            return returnValue;
        }

        private static (byte[] minColor, byte[] maxColor) GetMinMaxColorsWithAlpha(
            byte[] data, int startX, int startY)
        {
            byte[] minColor = [255, 255, 255, 255];
            byte[] maxColor = [0, 0, 0, 0];
            var color = new byte[4];

            for (int x = startX; x < startX + 4; x++)
            {
                for (int y = startY; y < startY + 4; y++)
                {
                    Buffer.BlockCopy(data, x*y, color, 0, 4);
                    if (color[0] < minColor[0]) { minColor[0] = color[0]; }
                    if (color[1] < minColor[1]) { minColor[1] = color[1]; }
                    if (color[2] < minColor[2]) { minColor[2] = color[2]; }
                    if (color[3] < minColor[3]) { minColor[3] = color[3]; }
                    if (color[0] > maxColor[0]) { maxColor[0] = color[0]; }
                    if (color[1] > maxColor[1]) { maxColor[1] = color[1]; }
                    if (color[2] > maxColor[2]) { maxColor[2] = color[2]; }
                    if (color[3] > maxColor[3]) { maxColor[3] = color[3]; }
                }
            }
            return (minColor, maxColor);
        }
    }
}
