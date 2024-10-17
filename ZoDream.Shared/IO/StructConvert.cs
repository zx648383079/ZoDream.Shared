using System.IO;
using System.Runtime.InteropServices;

namespace ZoDream.Shared.IO
{
    public static class StructConvert
    {

        public static byte[] ToByte<T>(T data) where T : struct
        {
            var size = SizeOf<T>();
            var buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(data, buffer, false);
                var res = new byte[size];
                Marshal.Copy(buffer, res, 0, size);
                return res;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static T ToStruct<T>(byte[] buffer) where T : struct
        {
            var size = SizeOf<T>();
            var handle = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buffer, 0, handle, size);
                return (T)Marshal.PtrToStructure(handle, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(handle);
            }
        }
        public static T ToStruct<T>(Stream input) where T : struct
        {
            var size = SizeOf<T>();
            var buffer = new byte[size];
            input.Read(buffer, 0, size);
            return ToStruct<T>(buffer);
        }


        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf<T>();
        }
    }
}
