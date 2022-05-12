using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Http
{
    public class RestInterceptor : IRequestInterceptor
    {
        public string ApiEndpoint { get; private set; }

        public string AppId { get; private set; }

        public string Secret { get; private set; }

        public RestInterceptor(string apiEndpoint, string appId, string secret)
        {
            ApiEndpoint = apiEndpoint;
            AppId = appId;
            Secret = secret;
        }

        public string Token { get; set; }

        public IHttpClient Request(IHttpClient client)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var headers = new Dictionary<string, string>
            {
                { "Date", timestamp },
                { "Content-Type", "application/vnd.api+json" },
                { "Accept", "application/json" },
                {  "HTTP_USER_AGENT", "zodream/5.0 UWPTimer/2.0" }
            };
            //if (GlobalizationPreferences.Languages.Count > 0)
            //{
            //    headers.Add("Accept-Language", GlobalizationPreferences.Languages[0]);
            //}
            if (!string.IsNullOrEmpty(Token))
            {
                headers.Add("Authorization", "Bearer " + Token);
            }
            client.Url = ApiEndpoint;
            client.Headers = headers;
            // client.AddQuery("appid", AppId).AddQuery("timestamp", timestamp)
                //.AddQuery("sign", Str.MD5Encode(AppId + timestamp + Secret));
            return client;
        }

        public T Response<T>(object data)
        {
            return (T)data;
        }

        public T Response<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public HttpException ResponseFailure(HttpException ex)
        {
            return ex;
        }
    }
}
