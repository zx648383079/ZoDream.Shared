﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

        public virtual string? Token { get; set; }

        public virtual IHttpClient Request(IHttpClient client)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var headers = new Dictionary<string, string>
            {
                { "Date", timestamp },
                { "Content-Type", "application/vnd.api+json" },
                { "Accept", "application/json" },
                {  "HTTP_USER_AGENT", "zodream/5.0" }
            };
            //if (GlobalizationPreferences.Languages.Count > 0)
            //{
            //    headers.Add("Accept-Language", GlobalizationPreferences.Languages[0]);
            //}
            if (!string.IsNullOrEmpty(Token))
            {
                headers.Add("Authorization", "Bearer " + Token);
            }
            var path = client.Url;
            client.Url = ApiEndpoint;
            client.Headers = headers;
            RestRequest.AppendPath(client, path, new Dictionary<string, string>
            {
                {"appid", AppId },
                {"timestamp", timestamp },
                {"sign", MD5Encode(AppId + timestamp + Secret) }
            });
            return client;
        }

        public virtual T Response<T>(object data)
        {
            return (T)data;
        }

        public virtual T? Response<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public virtual HttpException ResponseFailure(HttpException ex)
        {
            return ex;
        }

        public static string MD5Encode(string source)
        {
            var sor = Encoding.UTF8.GetBytes(source);
            var md5 = MD5.Create();
            var result = md5.ComputeHash(sor);
            md5.Dispose();
            var sb = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return sb.ToString();
        }
    }
}
