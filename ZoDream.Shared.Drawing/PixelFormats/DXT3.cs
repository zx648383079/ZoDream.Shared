using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class DXT3 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            byte[] buffer = new byte[width * height * 4];
            int xBlocks = width / 4;
            int yBlocks = height / 4;
            for (int y = 0; y < yBlocks; y++)
            {
                for (int x = 0; x < xBlocks; x++)
                {
                    int blockDataStart = ((y * xBlocks) + x) * 16;
                    ushort[] alphaData = new ushort[4];

                    alphaData[0] = (ushort)((data[blockDataStart + 1] << 8) + data[blockDataStart + 0]);
                    alphaData[1] = (ushort)((data[blockDataStart + 3] << 8) + data[blockDataStart + 2]);
                    alphaData[2] = (ushort)((data[blockDataStart + 5] << 8) + data[blockDataStart + 4]);
                    alphaData[3] = (ushort)((data[blockDataStart + 7] << 8) + data[blockDataStart + 6]);

                    byte[,] alpha = new byte[4, 4];
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            alpha[i, j] = (byte)((alphaData[j] & 0xF) * 16);
                            alphaData[j] >>= 4;
                        }
                    }

                    ushort color0 = (ushort)((data[blockDataStart + 9] << 8) + data[blockDataStart + 8]);
                    ushort color1 = (ushort)((data[blockDataStart + 11] << 8) + data[blockDataStart + 10]);

                    uint code = (uint)((data[blockDataStart + 8 + 6] << 24) + (data[blockDataStart + 8 + 7] << 16) + (data[blockDataStart + 8 + 4] << 8) + (data[blockDataStart + 8 + 5] << 0));

                    ushort r0 = 0, g0 = 0, b0 = 0, r1 = 0, g1 = 0, b1 = 0;
                    r0 = (ushort)(8 * (color0 & 31));
                    g0 = (ushort)(4 * ((color0 >> 5) & 63));
                    b0 = (ushort)(8 * ((color0 >> 11) & 31));

                    r1 = (ushort)(8 * (color1 & 31));
                    g1 = (ushort)(4 * ((color1 >> 5) & 63));
                    b1 = (ushort)(8 * ((color1 >> 11) & 31));

                    for (int k = 0; k < 4; k++)
                    {
                        int j = k ^ 1;
                        for (int i = 0; i < 4; i++)
                        {
                            int pixDataStart = (width * (y * 4 + j) * 4) + ((x * 4 + i) * 4);
                            uint codeDec = code & 0x3;

                            buffer[pixDataStart + 3] = alpha[i, j];

                            switch (codeDec)
                            {
                                case 0:
                                    buffer[pixDataStart + 0] = (byte)r0;
                                    buffer[pixDataStart + 1] = (byte)g0;
                                    buffer[pixDataStart + 2] = (byte)b0;
                                    break;
                                case 1:
                                    buffer[pixDataStart + 0] = (byte)r1;
                                    buffer[pixDataStart + 1] = (byte)g1;
                                    buffer[pixDataStart + 2] = (byte)b1;
                                    break;
                                case 2:
                                    if (color0 > color1)
                                    {
                                        buffer[pixDataStart + 0] = (byte)((2 * r0 + r1) / 3);
                                        buffer[pixDataStart + 1] = (byte)((2 * g0 + g1) / 3);
                                        buffer[pixDataStart + 2] = (byte)((2 * b0 + b1) / 3);
                                    }
                                    else
                                    {
                                        buffer[pixDataStart + 0] = (byte)((r0 + r1) / 2);
                                        buffer[pixDataStart + 1] = (byte)((g0 + g1) / 2);
                                        buffer[pixDataStart + 2] = (byte)((b0 + b1) / 2);
                                    }
                                    break;
                                case 3:
                                    if (color0 > color1)
                                    {
                                        buffer[pixDataStart + 0] = (byte)((r0 + 2 * r1) / 3);
                                        buffer[pixDataStart + 1] = (byte)((g0 + 2 * g1) / 3);
                                        buffer[pixDataStart + 2] = (byte)((b0 + 2 * b1) / 3);
                                    }
                                    else
                                    {
                                        buffer[pixDataStart + 0] = 0;
                                        buffer[pixDataStart + 1] = 0;
                                        buffer[pixDataStart + 2] = 0;
                                    }
                                    break;
                            }

                            code >>= 2;
                        }
                    }
                }
            }
            return buffer;
        }

        public byte[] Encode(byte[] data, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
