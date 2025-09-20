using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public partial class Client
    {

        private string ReadCharset(HttpResponseMessage response)
        {
            var items = response.Content.Headers.GetValues("Content-Type");
            foreach (var item in items)
            {
                var i = item.IndexOf("charset=");
                if (i < 0)
                {
                    continue;
                }
                return item.Substring(i + 7);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取HTML网页的编码
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public Encoding GetEncoding(byte[] bytes, string charSet)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var html = Encoding.Default.GetString(bytes);
            var match = Regex.Match(html, @"charset\b\s*=\s*""*([\da-zA-Z\-]*)");
            try
            {
                if (match.Success)
                {
                    return Encoding.GetEncoding(match.Groups[1].Value);
                }
                if (!string.IsNullOrWhiteSpace(charSet))
                {
                    return Encoding.GetEncoding(charSet);
                }
            }
            catch { }
            return Encoding.Default;
        }

        public Task<string?> GetAsync(string url)
        {
            Url = url;
            return ReadAsync();
        }

        public Task<string?> PostAsync(string url, string data)
        {
            Url = url;
            Body = data;
            Method = RequestMethod.Post;
            return ReadAsync();
        }

        public Task<string?> PostAsync(string url, string data, string contentType)
        {
            Headers.Add(ContentTypeKey, contentType);
            return PostAsync(url, data);
        }

        public Task<string?> PostAsync(string url, HttpContent data)
        {
            Url = url;
            Body = data;
            Method = RequestMethod.Post;
            return ReadAsync();
        }

        public async Task<string?> ReadAsync()
        {
            return await ReadAsync(ReadAsync);
        }

        public async Task<T?> ReadAsync<T>(Func<HttpResponseMessage, Task<T?>> func)
        {
            using (var request = PrepareRequest())
            using (var client = PrepareClient())
            using (var response = await client.SendAsync(request))
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return default;
                }
                return await func.Invoke(response);
            }
        } 

        public async Task<string?> ReadAsync(HttpResponseMessage response)
        {
            #region 判断解压
            using var stream = await ReadStreamAsync(response);
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
            #endregion
            var bytes = ms.ToArray();
            var html = GetEncoding(bytes, ReadCharset(response)).GetString(bytes);
            stream.Close();
            #endregion
            return html;
        }

        public async Task<T?> ReadJsonAsync<T>()
        {
            var content = await ReadAsync();
            if (content == null)
            {
                return default;
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }
            try
            {
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception)
            {
            }
            return default;
        }


        public async Task<HttpResponseMessage?> ReadResponseAsync()
        {
            using var request = PrepareRequest();
            using var client = PrepareClient();
            return await client.SendAsync(request);
        }

        public async Task<Stream?> ReadStreamAsync()
        {
            var response = await ReadResponseAsync();
            return await ReadStreamAsync(response);
        }

        public async Task<Stream?> ReadStreamAsync(HttpResponseMessage? response)
        {
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                return new GZipStream(await response.Content.ReadAsStreamAsync(), mode: CompressionMode.Decompress);
            }
            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// 获取内容长度
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetLengthAsync()
        {
            try
            {
                using var response = await ReadResponseAsync();
                return GetContentLength(response);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private long GetContentLength(HttpResponseMessage? response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return 0;
            }
            var length = response.Content.Headers.ContentLength;
            if (length == null)
            {
                return 0;
            }
            return (long)length;
        }

    }
}
