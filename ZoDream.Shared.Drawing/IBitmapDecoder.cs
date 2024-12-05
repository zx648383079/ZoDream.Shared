using System.IO;

namespace ZoDream.Shared.Drawing
{
    public interface IBitmapDecoder
    {
        public IImageData Decode(Stream input);
        public IImageData Decode(byte[] data);
        public IImageData Decode(string fileName);
        public byte[] Encode(IImageData data);
        public void Encode(IImageData data, Stream output);
        public void Encode(IImageData data, string fileName);
    }
}
