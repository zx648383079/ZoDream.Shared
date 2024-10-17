using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Drawing
{
    public interface IBufferDecoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>返回RGBA</returns>
        public byte[] Decode(byte[] data, int width, int height);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">RGBA</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] Encode(byte[] data, int width, int height);
    }
}
