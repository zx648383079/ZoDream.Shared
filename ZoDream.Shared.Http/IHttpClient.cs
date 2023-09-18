using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Http.Models;

namespace ZoDream.Shared.Http
{
    public interface IHttpClient: IDisposable
    {

        public string Url { get; set; }

        public RequestMethod Method { get; set; }

        public ProxyItem? Proxy { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public object? Body { get; set; }

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; }


        public Task<string?> GetAsync(string url);

        public Task<string?> PostAsync(string url, string data);

        public Task<string?> PostAsync(string url, HttpContent data);


        public Task<string?> ReadAsync();

        public Task<string?> ReadAsync(HttpResponseMessage response);

        public Task<T?> ReadJsonAsync<T>();

        public Task<Stream?> ReadStreamAsync();

        public Task<HttpResponseMessage?> ReadResponseAsync();

        public Task<long> GetLengthAsync();
        /// <summary>
        /// 下载整个文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> SaveAsync(string file,
            Action<long, long>? progress = null, CancellationToken token = default);

        public Task<bool> SaveAsync(HttpResponseMessage response, string file,
            Action<long, long>? progress = null, CancellationToken token = default);
        /// <summary>
        /// 获取一段文件，断点续传用，请先使用 GetLengthAsync() 获取内容大小，进行判断
        /// </summary>
        /// <param name="file"></param>
        /// <param name="current"></param>
        /// <param name="maxSize">本次下载的最大长度</param>
        /// <param name="appendFile"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> SaveAsync(string file,
            long current, long maxSize = 512000,
            bool appendFile = true,
            Action<long, long>? progress = null, CancellationToken token = default);



    }
}
