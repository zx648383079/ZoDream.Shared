using System;
using System.Collections.Specialized;
using System.Numerics;

namespace ZoDream.Shared.Drawing.PVRTC
{
    /// <summary>
    /// @see https://github.com/mcraiha/CSharp-PVRTC-encdec
    /// </summary>
    public sealed class Packet
    {
        public static readonly byte[][] BILINEAR_FACTORS =
        {
            [4, 4, 4, 4],
            [2, 6, 2, 6],
            [ 8, 0, 8, 0 ],

            [6, 2, 6, 2 ],


            [2, 2, 6, 6 ],
            [1, 3, 3, 9 ],
            [ 4, 0, 12, 0 ],
            [ 3, 1, 9, 3 ],

            [ 8, 8, 0, 0 ],
            [ 4, 12, 0, 0 ],
            [ 16, 0, 0, 0 ],
            [ 12, 4, 0, 0 ],

            [ 6, 6, 2, 2 ],
            [ 3, 9, 1, 3 ],
            [ 12, 0, 4, 0 ],
            [ 9, 3, 3, 1 ],
        };

        public static readonly byte[][] WEIGHTS =
        {
			// Weights for Mode=0
			[ 8, 0, 8, 0 ],
            [ 5, 3, 5, 3 ],
            [ 3, 5, 3, 5 ],
            [ 0, 8, 0, 8 ],
			
			// Weights for Mode=1
			[ 8, 0, 8, 0 ],
            [ 4, 4, 4, 4 ],
            [ 4, 4, 0, 0 ],
            [ 0, 8, 0, 8 ],
        };

        uint _modulationData;
        bool _usePunchThroughAlpha;
        bool _colorAIsOpaque;
        bool _colorBIsOpaque;
        uint _colorA;
        uint _colorB;

        public Packet()
        {
            // Default constructor doesn't do anything
        }

        public uint GetModulationData()
        {
            return _modulationData;
        }

        public void SetModulationData(uint newValue)
        {
            _modulationData = newValue;
        }

        public int GetPunchThroughAlpha()
        {
            if (_usePunchThroughAlpha)
            {
                return 1;
            }
            return 0;
        }

        public void SetPunchThroughAlpha(bool newValue)
        {
            _usePunchThroughAlpha = newValue;
        }

        public bool GetColorAIsOpaque()
        {
            return _colorAIsOpaque;
        }

        public bool GetColorBIsOpaque()
        {
            return _colorBIsOpaque;
        }

        public uint GetColorA()
        {
            return _colorA;
        }

        public uint GetColorB()
        {
            return _colorB;
        }

        // Gets A color as RGB int
        public Vector3 GetColorRgbA()
        {
            if (_colorAIsOpaque)
            {
                byte r = (byte)(_colorA >> 9);
                byte g = (byte)(_colorA >> 4 & 0x1f);
                byte b = (byte)(_colorA & 0xf);

                return new Vector3(BitScale.BITSCALE_5_TO_8[r],
                                    BitScale.BITSCALE_5_TO_8[g],
                                    BitScale.BITSCALE_4_TO_8[b]);
            }
            else
            {
                byte r = (byte)(_colorA >> 7 & 0xf);
                byte g = (byte)(_colorA >> 3 & 0xf);
                byte b = (byte)(_colorA & 7);

                return new Vector3(BitScale.BITSCALE_4_TO_8[r],
                                    BitScale.BITSCALE_4_TO_8[g],
                                    BitScale.BITSCALE_3_TO_8[b]);
            }
        }

        // Gets B color as RGB int
        public Vector3 GetColorRgbB()
        {
            if (_colorBIsOpaque)
            {
                byte r = (byte)(_colorB >> 10);
                byte g = (byte)(_colorB >> 5 & 0x1f);
                byte b = (byte)(_colorB & 0x1f);

                return new Vector3(BitScale.BITSCALE_5_TO_8[r],
                                    BitScale.BITSCALE_5_TO_8[g],
                                    BitScale.BITSCALE_5_TO_8[b]);
            }
            else
            {
                byte r = (byte)(_colorB >> 8 & 0xf);
                byte g = (byte)(_colorB >> 4 & 0xf);
                byte b = (byte)(_colorB & 0xf);

                return new Vector3(BitScale.BITSCALE_4_TO_8[r],
                                    BitScale.BITSCALE_4_TO_8[g],
                                    BitScale.BITSCALE_4_TO_8[b]);
            }
        }

        // Gets A color as RGBA int
        public Vector4 GetColorRgbaA()
        {
            if (_colorAIsOpaque)
            {
                byte r = (byte)(_colorA >> 9);
                byte g = (byte)(_colorA >> 4 & 0x1f);
                byte b = (byte)(_colorA & 0xf);

                return new Vector4(BitScale.BITSCALE_5_TO_8[r],
                                    BitScale.BITSCALE_5_TO_8[g],
                                    BitScale.BITSCALE_4_TO_8[b],
                                    255);
            }
            else
            {
                byte a = (byte)(_colorA >> 11 & 7);
                byte r = (byte)(_colorA >> 7 & 0xf);
                byte g = (byte)(_colorA >> 3 & 0xf);
                byte b = (byte)(_colorA & 7);

                return new Vector4(BitScale.BITSCALE_4_TO_8[r],
                                    BitScale.BITSCALE_4_TO_8[g],
                                    BitScale.BITSCALE_3_TO_8[b],
                                    BitScale.BITSCALE_3_TO_8[a]);
            }
        }

        // Gets B color as RGBA int
        public Vector4 GetColorRgbaB()
        {
            if (_colorBIsOpaque)
            {
                byte r = (byte)(_colorB >> 10);
                byte g = (byte)(_colorB >> 5 & 0x1f);
                byte b = (byte)(_colorB & 0x1f);

                return new Vector4(BitScale.BITSCALE_5_TO_8[r],
                                    BitScale.BITSCALE_5_TO_8[g],
                                    BitScale.BITSCALE_5_TO_8[b],
                                    255);
            }
            else
            {
                byte a = (byte)(_colorB >> 12 & 7);
                byte r = (byte)(_colorB >> 8 & 0xf);
                byte g = (byte)(_colorB >> 4 & 0xf);
                byte b = (byte)(_colorB & 0xf);

                return new Vector4(BitScale.BITSCALE_4_TO_8[r],
                                    BitScale.BITSCALE_4_TO_8[g],
                                    BitScale.BITSCALE_4_TO_8[b],
                                    BitScale.BITSCALE_3_TO_8[a]);
            }
        }

        // Set color A, NO alpha
        public void SetColorA(byte rr, byte gg, byte bb)
        {
            int r = BitScale.BITSCALE_8_TO_5_FLOOR[rr];
            int g = BitScale.BITSCALE_8_TO_5_FLOOR[gg];
            int b = BitScale.BITSCALE_8_TO_4_FLOOR[bb];

            _colorA = (uint)(r << 9 | g << 4 | b);
            _colorAIsOpaque = true;
        }

        // Set color B, NO alpha
        public void SetColorB(byte rr, byte gg, byte bb)
        {
            int r = BitScale.BITSCALE_8_TO_5_CEIL[rr];
            int g = BitScale.BITSCALE_8_TO_5_CEIL[gg];
            int b = BitScale.BITSCALE_8_TO_5_CEIL[bb];

            _colorB = (uint)(r << 10 | g << 5 | b);
            _colorBIsOpaque = true;
        }

        // Set color A with alpha
        public void SetColorA(byte rr, byte gg, byte bb, byte aa)
        {
            int a = BitScale.BITSCALE_8_TO_3_FLOOR[aa];
            if (a == 7)
            {
                int r = BitScale.BITSCALE_8_TO_5_FLOOR[rr];
                int g = BitScale.BITSCALE_8_TO_5_FLOOR[gg];
                int b = BitScale.BITSCALE_8_TO_4_FLOOR[bb];

                _colorA = (uint)(r << 9 | g << 4 | b);
                _colorAIsOpaque = true;
            }
            else
            {
                int r = BitScale.BITSCALE_8_TO_4_FLOOR[rr];
                int g = BitScale.BITSCALE_8_TO_4_FLOOR[gg];
                int b = BitScale.BITSCALE_8_TO_3_FLOOR[bb];

                _colorA = (uint)(a << 11 | r << 7 | g << 3 | b);
                _colorAIsOpaque = false;
            }
        }

        // Set color B with alpha
        public void SetColorB(byte rr, byte gg, byte bb, byte aa)
        {
            int a = BitScale.BITSCALE_8_TO_3_CEIL[aa];
            if (a == 7)
            {
                int r = BitScale.BITSCALE_8_TO_5_CEIL[rr];
                int g = BitScale.BITSCALE_8_TO_5_CEIL[gg];
                int b = BitScale.BITSCALE_8_TO_5_CEIL[bb];

                _colorB = (uint)(r << 10 | g << 5 | b);
                _colorBIsOpaque = true;
            }
            else
            {
                int r = BitScale.BITSCALE_8_TO_4_CEIL[rr];
                int g = BitScale.BITSCALE_8_TO_4_CEIL[gg];
                int b = BitScale.BITSCALE_8_TO_4_CEIL[bb];

                _colorB = (uint)(a << 12 | r << 8 | g << 4 | b);
                _colorBIsOpaque = false;
            }
        }

        public byte[] GetAsByteArray()
        {
            var returnValue = new byte[8];
            var modulationDataByteArray = BitConverter.GetBytes(_modulationData);

            var tempBitVector = new BitVector32(0);
            int currentIndex = 0;
            tempBitVector[1 << currentIndex] = _usePunchThroughAlpha;
            currentIndex++;

            var tempBitVectorColorA = new BitVector32((int)_colorA);
            for (int i = 0; i < 14; i++)
            {
                tempBitVector[1 << currentIndex] = tempBitVectorColorA[1 << i];
                currentIndex++;
            }

            tempBitVector[1 << currentIndex] = _colorAIsOpaque;
            currentIndex++;

            var tempBitVectorColorB = new BitVector32((int)_colorB);
            for (int i = 0; i < 15; i++)
            {
                tempBitVector[1 << currentIndex] = tempBitVectorColorB[1 << i];
                currentIndex++;
            }

            tempBitVector[1 << currentIndex] = _colorBIsOpaque;
            currentIndex++;

            Buffer.BlockCopy(modulationDataByteArray, 0, returnValue, 0, 4);
            byte[] otherDataByteArray = BitConverter.GetBytes(tempBitVector.Data);
            Buffer.BlockCopy(otherDataByteArray, 0, returnValue, 4, 4);

            return returnValue;
        }

        public void InitFromBytes(byte[] data)
        {
            byte[] modulationDataByteArray = new byte[4];
            byte[] otherDataByteArray = new byte[4];
            Buffer.BlockCopy(data, 0, modulationDataByteArray, 0, 4);
            Buffer.BlockCopy(data, 4, otherDataByteArray, 0, 4);

            _modulationData = BitConverter.ToUInt32(modulationDataByteArray, 0);
            var tempBitVector = new BitVector32(BitConverter.ToInt32(otherDataByteArray, 0));

            var punchThroughAlphaSection = BitVector32.CreateSection(1);
            var colorASection = BitVector32.CreateSection(16383 /*(1 << 14) - 1*/, punchThroughAlphaSection);
            var colorAIsOpaque = BitVector32.CreateSection(1, colorASection);
            var colorBSection = BitVector32.CreateSection(32767 /*(1 << 15) - 1*/, colorAIsOpaque);
            var colorBIsOpaque = BitVector32.CreateSection(1, colorBSection);

            if (tempBitVector[punchThroughAlphaSection] == 1)
            {
                _usePunchThroughAlpha = true;
            }

            _colorA = (uint)tempBitVector[colorASection];

            if (tempBitVector[colorAIsOpaque] == 1)
            {
                _colorAIsOpaque = true;
            }

            _colorB = (uint)tempBitVector[colorBSection];

            if (tempBitVector[colorBIsOpaque] == 1)
            {
                _colorBIsOpaque = true;
            }
        }
    }
}