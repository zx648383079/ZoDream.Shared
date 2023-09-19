using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public partial class Client
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="progress">下载进度，progress, total</param>
        /// <returns></returns>
        public async Task<bool> SaveAsync(string file, 
            Action<long, long>? progress = null, CancellationToken token = default)
        {
            var response = await ReadResponseAsync();
            if (response == null)
            {
                return false;
            }
            return await SaveAsync(response, file, progress, token);
        }

        public async Task<bool> SaveAsync(HttpResponseMessage response, string file,
            Action<long, long>? progress = null, CancellationToken token = default)
        {
            var length = GetContentLength(response);
            if (length <= 0 || token.IsCancellationRequested)
            {
                return false;
            }
            using var responseStream = await ReadStreamAsync(response);
            if (responseStream == null)
            {
                return false;
            }
            using var stream = new FileStream(file, FileMode.Create);
            var bArr = new byte[1024];
            var byteReceived = 0L;
            progress?.Invoke(byteReceived, length);
            int size;
            do
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                size = responseStream.Read(bArr, 0, bArr.Length);
                if (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    byteReceived += size;
                    progress?.Invoke(byteReceived, length);
                }
            } while (size > 0);
            return true;
        }

        /// <summary>
        /// 断点续下载
        /// </summary>
        /// <param name="file"></param>
        /// <param name="current"></param>
        /// <param name="maxSize">本次下载的最大长度</param>
        /// <param name="appendFile"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> SaveAsync(string file, 
            long current, long maxSize = 512000,
            bool appendFile = true,
            Action<long, long>? progress = null, CancellationToken token = default)
        {
            try
            {
                using var request = PrepareRequest();
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(current, 
                    current + maxSize - 1);
                using var client = PrepareClient();
                using var response = await client.SendAsync(request);
                // 重新下载
                //if (!response.Content.Headers.ContentRange.HasRange)
                //{
                //    // 不接受指定范围
                //    // return false;
                //    current = 0;
                //}
                current = response.Content.Headers.ContentRange?.From ?? 0;
                var length = GetContentLength(response);
                if (length <= 0)
                {
                    return false;
                }
                using var responseStream = await ReadStreamAsync(response);
                if (responseStream is null)
                {
                    return false;
                }
                //创建本地文件写入流
                var stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                stream.Seek(appendFile ? current : 0, SeekOrigin.Begin);
                var bArr = new byte[1024];
                var byteReceived = 0L;
                progress?.Invoke(current + byteReceived, current +  length);
                int size;
                do
                {
                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }
                    size = responseStream.Read(bArr, 0, bArr.Length);
                    if (size > 0)
                    {
                        stream.Write(bArr, 0, size);
                        byteReceived += size;
                        progress?.Invoke(current + byteReceived, current + length);
                    }
                } while (size > 0);
                if (!appendFile)
                {
                    stream.Flush();
                    stream.SetLength(stream.Position);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<MemoryStream?> ReadMemoryStreamAsync(HttpResponseMessage? response)
        {

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            #region 判断解压
            var stream = await ReadStreamAsync(response);
            #endregion
            #region 把网络流转成内存流
            var ms = new MemoryStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream == null) continue;
                var sz = stream.Read(buffer, 0, 1024);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            stream.Close();
            #endregion
            return ms;
        }

        public async Task<MemoryStream?> ReadMemoryStreamAsync()
        {
            var response = await ReadResponseAsync();
            return await ReadMemoryStreamAsync(response);
        }

    }
}
