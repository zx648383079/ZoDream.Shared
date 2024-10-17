using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public class CTX1(bool swapXY = false, bool computeZ = true) : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var buffer = new byte[width * height * 4];
            int xBlocks = width / 4;
            int yBlocks = height / 4;

            var vectors = new RGBAColor[4];
            for (int i = 0; i < yBlocks; i++)
            {
                for (int j = 0; j < xBlocks; j++)
                {
                    int srcIndex = (i * xBlocks + j) * 8;
                    vectors[0] = new RGBAColor(data[srcIndex + 1], data[srcIndex + 0], 0, 0);
                    vectors[1] = new RGBAColor(data[srcIndex + 3], data[srcIndex + 2], 0, 0);
                    vectors[2].R = (byte)((2 * vectors[0].R + vectors[1].R + 1) / 3);
                    vectors[2].G = (byte)((2 * vectors[0].G + vectors[1].G + 1) / 3);
                    vectors[3].R = (byte)((vectors[0].R + 2 * vectors[1].R + 1) / 3);
                    vectors[3].G = (byte)((vectors[0].G + 2 * vectors[1].G + 1) / 3);

                    var code = (uint)((data[srcIndex + 7] << 24) | (data[srcIndex + 6] << 16) | (data[srcIndex + 5] << 8) | (data[srcIndex + 4]));

                    for (int k = 0; k < 4; k++)
                    {
                        for (int m = 0; m < 4; m++)
                        {
                            int destIndex = ((width * ((i * 4) + k)) * 4) + (((j * 4) + m) * 4);

                            RGBAColor vector = vectors[(int)(code & 3)];

                            RGBAColor color;
                            color.R = vector.R;
                            color.G = vector.G;
                            color.B = computeZ ? CalculateNormalZ(vector.R, vector.G) : (byte)0;
                            color.A = 0xFF;

                            if (swapXY)
                            {
                                (color.R, color.G) = (color.G, color.R);
                            }

                            buffer[destIndex + 0] = color.B;
                            buffer[destIndex + 1] = color.G;
                            buffer[destIndex + 2] = color.R;
                            buffer[destIndex + 3] = color.A;

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

        static byte CalculateNormalZ(byte r, float g)
        {
            float x = (r / 255f * 2f) - 1f;
            float y = (g / 255f * 2f) - 1f;
            float z = (float)Math.Sqrt(Math.Max(0f, Math.Min(1f, (1f - (x * x)) - (y * y))));
            return (byte)(((z + 1f) / 2f) * 255f);
        }
    }
}
