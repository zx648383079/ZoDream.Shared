using System;
using System.Net;
using System.Text;
using System.Net.Http;

namespace ZoDream.Shared.Http
{
    public partial class Client
    {

        protected HttpMessageHandler PrepareHandler()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = AllowAutoRedirect,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };
            //if (Cookies != null && Cookies.Count > 0)
            //{   
            //    handler.CookieContainer.Add(Cookies);
            //}
            var uri = new Uri(Url);
            if (Headers.ContainsKey("Cookie"))
            {
                foreach (var item in Headers["Cookie"].Split(';'))
                {
                    handler.CookieContainer.SetCookies(uri, item);
                }
            }
            if (Proxy != null)
            {
                // handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
                handler.Proxy = new WebProxy()
                {
                    Address = new Uri($"{Proxy.Schema}://{Proxy.Host}:{Proxy.Port}"),
                    UseDefaultCredentials = string.IsNullOrWhiteSpace(Proxy.UserName),
                    Credentials = string.IsNullOrWhiteSpace(Proxy.UserName) ? null : new NetworkCredential(
                    userName: Proxy.UserName,
                    password: Proxy.Password)
                };
            }
            if (MaxRetries > 1)
            {
                return new HttpRetryHandler(handler, MaxRetries, RetryTime);
            }
            return handler;
        }

        public HttpClient PrepareClient()
        {
            var client = new HttpClient(PrepareHandler())
            {
                Timeout = TimeSpan.FromMilliseconds(TimeOut)
            };
            return client;
        }

        public HttpRequestMessage PrepareRequest()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(Url),
                Method = FormatMethod()
            };
            foreach (var item in Headers)
            {
                request.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
            if (Method == RequestMethod.Post)
            {
                if (Body is null)
                {
                }
                else if (Body is JsonStringContent json)
                {
                    request.Content = json.ToHttpContent();
                }
                else if (Body is HttpContent body)
                {
                    request.Content = body;
                } else if (Headers.ContainsKey(ContentTypeKey))
                {
                    request.Content = new StringContent(Body!.ToString(), Encoding.UTF8, Headers[ContentTypeKey]);
                } else
                {
                    request.Content = new StringContent(Body!.ToString());
                }
            }
            return request;
        }

        private HttpMethod FormatMethod()
        {
            return Method switch
            {
                RequestMethod.Post => HttpMethod.Post,
                RequestMethod.Put => HttpMethod.Put,
                RequestMethod.Delete => HttpMethod.Delete,
                RequestMethod.Head => HttpMethod.Head,
                RequestMethod.Options => HttpMethod.Options,
                RequestMethod.Trace => HttpMethod.Trace,
                _ => HttpMethod.Get,
            };
        }


    }
}
