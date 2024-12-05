using System.IO;

namespace ZoDream.Shared.Drawing
{
    public abstract class BaseBitmapDecoder : IBitmapDecoder
    {
        public virtual IImageData Decode(Stream input)
        {
            var buffer = new byte[input.Length - input.Position];
            input.ReadExactly(buffer);
            return Decode(buffer);
        }

        public abstract IImageData Decode(byte[] data);

        public IImageData Decode(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            return Decode(fs);
        }

        public abstract byte[] Encode(IImageData data);

        public virtual void Encode(IImageData data, Stream output)
        {
            output.Write(Encode(data));
        }

        public void Encode(IImageData data, string fileName)
        {
            using var fs = File.OpenWrite(fileName);
            Encode(data, fs);
        }
    }
}
