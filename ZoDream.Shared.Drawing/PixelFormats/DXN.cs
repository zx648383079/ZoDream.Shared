using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    internal class DXN : IBufferDecoder
    {
        public byte[] Decode(byte[] data, int width, int height)
        {
            var size = width * height;
            var buffer = new byte[size * 4];
            var chunks = width / 4;

            if (chunks == 0)
            {
                chunks = 1;
            }

            for (int i = 0; i < size; i += 16)
            {
                byte rMin = data[i + 0];
                byte rMax = data[i + 1];
                byte[] rIndices = new byte[16];
                int temp = ((data[i + 4] << 16) | (data[i + 3] << 8)) | data[i + 2];
                int indices = 0;
                while (indices < 8)
                {
                    rIndices[indices] = (byte)(temp & 7);
                    temp = temp >> 3;
                    indices++;
                }
                temp = ((data[i + 7] << 16) | (data[i + 6] << 8)) | data[i + 5];
                while (indices < 16)
                {
                    rIndices[indices] = (byte)(temp & 7);
                    temp = temp >> 3;
                    indices++;
                }
                byte gMin = data[i + 8];
                byte gMax = data[i + 9];
                byte[] gIndices = new byte[16];
                temp = ((data[i + 12] << 16) | (data[i + 11] << 8)) | data[i + 10];
                indices = 0;
                while (indices < 8)
                {
                    gIndices[indices] = (byte)(temp & 7);
                    temp = temp >> 3;
                    indices++;
                }
                temp = ((data[i + 15] << 16) | (data[i + 14] << 8)) | data[i + 13];
                while (indices < 16)
                {
                    gIndices[indices] = (byte)(temp & 7);
                    temp = temp >> 3;
                    indices++;
                }
                byte[] redTable = new byte[8];
                redTable[0] = rMin;
                redTable[1] = rMax;
                if (redTable[0] > redTable[1])
                {
                    redTable[2] = (byte)((6 * redTable[0] + 1 * redTable[1]) / 7.0f);
                    redTable[3] = (byte)((5 * redTable[0] + 2 * redTable[1]) / 7.0f);
                    redTable[4] = (byte)((4 * redTable[0] + 3 * redTable[1]) / 7.0f);
                    redTable[5] = (byte)((3 * redTable[0] + 4 * redTable[1]) / 7.0f);
                    redTable[6] = (byte)((2 * redTable[0] + 5 * redTable[1]) / 7.0f);
                    redTable[7] = (byte)((1 * redTable[0] + 6 * redTable[1]) / 7.0f);
                }
                else
                {
                    redTable[2] = (byte)((4 * redTable[0] + 1 * redTable[1]) / 5.0f);
                    redTable[3] = (byte)((3 * redTable[0] + 2 * redTable[1]) / 5.0f);
                    redTable[4] = (byte)((2 * redTable[0] + 3 * redTable[1]) / 5.0f);
                    redTable[5] = (byte)((1 * redTable[0] + 4 * redTable[1]) / 5.0f);
                    redTable[6] = (byte)0;
                    redTable[7] = (byte)255;
                }
                byte[] grnTable = new byte[8];
                grnTable[0] = gMin;
                grnTable[1] = gMax;
                if (grnTable[0] > grnTable[1])
                {
                    grnTable[2] = (byte)((6 * grnTable[0] + 1 * grnTable[1]) / 7.0f);
                    grnTable[3] = (byte)((5 * grnTable[0] + 2 * grnTable[1]) / 7.0f);
                    grnTable[4] = (byte)((4 * grnTable[0] + 3 * grnTable[1]) / 7.0f);
                    grnTable[5] = (byte)((3 * grnTable[0] + 4 * grnTable[1]) / 7.0f);
                    grnTable[6] = (byte)((2 * grnTable[0] + 5 * grnTable[1]) / 7.0f);
                    grnTable[7] = (byte)((1 * grnTable[0] + 6 * grnTable[1]) / 7.0f);
                }
                else
                {
                    grnTable[2] = (byte)((4 * grnTable[0] + 1 * grnTable[1]) / 5.0f);
                    grnTable[3] = (byte)((3 * grnTable[0] + 2 * grnTable[1]) / 5.0f);
                    grnTable[4] = (byte)((2 * grnTable[0] + 3 * grnTable[1]) / 5.0f);
                    grnTable[5] = (byte)((1 * grnTable[0] + 4 * grnTable[1]) / 5.0f);
                    grnTable[6] = (byte)0;
                    grnTable[7] = (byte)255;
                }
                int chunkNum = i / 16;
                int xPos = chunkNum % chunks;
                int yPos = (chunkNum - xPos) / chunks;
                int sizeh = (height < 4) ? height : 4;
                int sizew = (width < 4) ? width : 4;
                for (int j = 0; j < sizeh; j++)
                {
                    for (int k = 0; k < sizew; k++)
                    {
                        RGBAColor color;
                        color.G = redTable[rIndices[(j * sizeh) + k]];
                        color.R = grnTable[gIndices[(j * sizeh) + k]];
                        float x = ((((float)color.R) / 255f) * 2f) - 1f;
                        float y = ((((float)color.G) / 255f) * 2f) - 1f;
                        float z = (float)Math.Sqrt((double)Math.Max(0f, Math.Min(1f, (1f - (x * x)) - (y * y))));
                        color.B = (byte)(((z + 1f) / 2f) * 255f);
                        color.A = 0xFF;
                        temp = (((((yPos * 4) + j) * width) + (xPos * 4)) + k) * 4;
                        buffer[temp] = (byte)color.B;
                        buffer[temp + 1] = (byte)color.G;
                        buffer[temp + 2] = (byte)color.R;
                        buffer[temp + 3] = (byte)color.A;
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
