using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class DXT1 : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            byte[] buffer = new byte[(width * height) * 4];
            int xBlocks = width / 4;
            int yBlocks = height / 4;
            for (int i = 0; i < yBlocks; i++)
            {
                for (int j = 0; j < xBlocks; j++)
                {
                    int index = ((i * xBlocks) + j) * 8;
                    uint colour0 = (uint)((data[index + 1] << 8) + data[index + 0]);
                    uint colour1 = (uint)((data[index + 3] << 8) + data[index + 2]);
                    uint code = (uint)((data[index + 7] << 24) + (data[index + 6] << 16) + (data[index + 5] << 8) + (data[index + 4] << 0));

                    ushort r0 = 0, g0 = 0, b0 = 0, r1 = 0, g1 = 0, b1 = 0;

                    r0 = (ushort)(8 * (colour0 & 0x1F));
                    g0 = (ushort)(4 * ((colour0 >> 5) & 0x3F));
                    b0 = (ushort)(8 * ((colour0 >> 11) & 0x1F));

                    r1 = (ushort)(8 * (colour1 & 0x1F));
                    g1 = (ushort)(4 * ((colour1 >> 5) & 0x3F));
                    b1 = (ushort)(8 * ((colour1 >> 11) & 0x1F));

                    for (int k = 0; k < 4; k++)
                    {
                        for (int m = 0; m < 4; m++)
                        {
                            int dataStart = ((width * ((i * 4) + k)) * 4) + (((j * 4) + m) * 4);
                            switch (code & 3)
                            {
                                case 0:
                                    buffer[dataStart] = (byte)r0;
                                    buffer[dataStart + 1] = (byte)g0;
                                    buffer[dataStart + 2] = (byte)b0;
                                    buffer[dataStart + 3] = 0xFF;
                                    break;

                                case 1:
                                    buffer[dataStart] = (byte)r1;
                                    buffer[dataStart + 1] = (byte)g1;
                                    buffer[dataStart + 2] = (byte)b1;
                                    buffer[dataStart + 3] = 0xFF;
                                    break;

                                case 2:
                                    buffer[dataStart + 3] = 0xFF;
                                    if (colour0 <= colour1)
                                    {
                                        buffer[dataStart] = (byte)((r0 + r1) / 2);
                                        buffer[dataStart + 1] = (byte)((g0 + g1) / 2);
                                        buffer[dataStart + 2] = (byte)((b0 + b1) / 2);
                                    }
                                    buffer[dataStart] = (byte)(((2 * r0) + r1) / 3);
                                    buffer[dataStart + 1] = (byte)(((2 * g0) + g1) / 3);
                                    buffer[dataStart + 2] = (byte)(((2 * b0) + b1) / 3);
                                    break;

                                case 3:
                                    if (colour0 <= colour1)
                                    {
                                        buffer[dataStart] = 0;
                                        buffer[dataStart + 1] = 0;
                                        buffer[dataStart + 2] = 0;
                                        buffer[dataStart + 3] = 0;
                                    }
                                    buffer[dataStart] = (byte)((r0 + (2 * r1)) / 3);
                                    buffer[dataStart + 1] = (byte)((g0 + (2 * g1)) / 3);
                                    buffer[dataStart + 2] = (byte)((b0 + (2 * b1)) / 3);
                                    buffer[dataStart + 3] = 0xFF;
                                    break;

                                default:
                                    break;
                            }
                            code = code >> 2;
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
