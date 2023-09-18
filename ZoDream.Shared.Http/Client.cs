using System.Collections.Generic;
using ZoDream.Shared.Http.Models;

namespace ZoDream.Shared.Http
{
    public partial class Client: IHttpClient
    {

        public const string ContentTypeKey = "Content-Type";
        public string Url { get; set; } = string.Empty;

        public RequestMethod Method { get; set; } = RequestMethod.Get;

        public bool AllowAutoRedirect { get; set; } = true;

        public ProxyItem? Proxy { get; set; }

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public object? Body { get; set; }

        /// <summary>
        /// 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; } = 5 * 1000;

        public int MaxRetries { get; set; } = 1;
        /// <summary>
        /// 重试间隔时间(/s)
        /// </summary>
        public int RetryTime { get; set; } = 0;

        public Client()
        {

        }

        public Client(string url)
        {
            Url = url;
        }

        public void Dispose()
        {
            Headers.Clear();
        }
    }
}
