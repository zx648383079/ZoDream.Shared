using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public class RestRequest
    {
        public RestRequest(IRequestInterceptor interceptor)
        {
            Interceptor = interceptor;
        }

        private readonly IRequestInterceptor Interceptor;

        /// <summary>
        /// Makes an HTTP GET request to the given controller and returns the deserialized response content.
        /// </summary>
        public async Task<TResult> GetAsync<TResult>(string controller, HttpExceptionFunc? action = null)
        {
            using var client = CreateHttp();
            AppendPath(client, controller);
            var obj = await ExecuteAsync<TResult>(client, action);
            return obj;
        }

        public async Task<TResult> GetAsync<TResult>(string controller, string key, object value, HttpExceptionFunc? action = null)
        {
            using var client = CreateHttp();
            AppendPath(client, controller, new Dictionary<string, string>()
            {
                {key, value.ToString()}
            });
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> GetAsync<TResult>(string controller, Dictionary<string, string> parameters, HttpExceptionFunc? action = null)
        {
            using var client = CreateHttp();
            AppendPath(client, controller, parameters);
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> GetAsync<TResult>(string controller, object parameters, HttpExceptionFunc? action = null)
        {
            using var client = CreateHttp();
            AppendPath(client, controller, parameters.GetType().GetProperties()
                .ToDictionary(q => q.Name, q => q.GetValue(parameters).ToString()));
            return await ExecuteAsync<TResult>(client, action);
        }

        /// <summary>
        /// Makes an HTTP POST request to the given controller with the given object as the body.
        /// Returns the deserialized response content.
        /// </summary>
        public async Task<TResult> PostAsync<TRequest, TResult>(string controller, TRequest body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            AppendPath(client, controller);
            client.Body = new JsonStringContent(body);
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> PostAsync<TResult>(string controller, object body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            AppendPath(client, controller);
            client.Body = new JsonStringContent(body);
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> PostAsync<TResult>(string controller, Dictionary<string, string> body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            AppendPath(client, controller);
            client.Body = new JsonStringContent(body);
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> PostAsync<TResult>(string controller, HttpContent body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            AppendPath(client, controller);
            client.Body = body;
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> PostAsync<TResult>(string controller, string body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            AppendPath(client, controller);
            client.Body = JsonStringContent.ParseJson(body);
            return await ExecuteAsync<TResult>(client, action);
        }

        /// <summary>
        /// Makes an HTTP DELETE request to the given controller and includes all the given
        /// object's properties as URL parameters. Returns the deserialized response content.
        /// </summary>
        public async Task<TResult> DeleteAsync<TResult>(string controller, uint objectId, HttpExceptionFunc? action = null)
        {
            using var client = CreateHttp();
            client.Method = RequestMethod.Delete;
            AppendPath(client, $"{controller}/{objectId}");
            return await ExecuteAsync<TResult>(client, action);
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string controller, TRequest body, HttpExceptionFunc? action = null)
        {
            using var client = CreatePostHttp();
            client.Body = new JsonStringContent(body);
            AppendPath(client, controller);
            client.Method = RequestMethod.Put;
            var obj = await ExecuteAsync<TResult>(client, action);
            return obj;
        }

        private async Task<T> ExecuteAsync<T>(IHttpClient client, HttpExceptionFunc? func)
        {
            try
            {
                using var response = await client.ReadResponseAsync();
                if (response == null)
                {
                    func?.Invoke(new HttpException());
                    return default(T);
                }
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var err = content.IndexOf("<html") >= 0 ? new HttpException((int)response.StatusCode, 
                        content) : JsonConvert.DeserializeObject<HttpException>(content);
                    func?.Invoke(err ?? new HttpException());
                    return default(T);
                }
                if (content is null)
                {
                    return default(T);
                }
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)content;
                }
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                func?.Invoke(new HttpException(ex.Message));
            }
            return default(T);
        }

        protected IHttpClient AppendPath(IHttpClient client, string path, IDictionary<string, string>? queries = null)
        {
            var uri = client.Url;
            if (string.IsNullOrEmpty(uri))
            {
                uri = path;
            } else
            {
                uri = uri.TrimEnd('/') + "/" + path.TrimStart('/');
            }
            client.Url = AppendQeuries(uri, queries);
            return client;
        }

        /// <summary>
        /// Constructs the base HTTP client, including correct authorization and API version headers.
        /// </summary>
        protected IHttpClient CreateHttp()
        {
            return Interceptor.Request(new Client());
        }

        protected string AppendQeuries(string path, IDictionary<string, string>? queries)
        {
            var query = BuildQueries(queries);
            if (string.IsNullOrEmpty(query))
            {
                return path;
            }
            if (path.Contains("?"))
            {
                return path + "&" + query;
            }
            return path + "?" + query;
        }

        protected string BuildQueries(IDictionary<string, string>? queries)
        {
            if (queries == null || !queries.Any()) return string.Empty;
            var builder = new StringBuilder();
            foreach (var content in queries)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }
                builder.Append($"{System.Net.WebUtility.UrlEncode(content.Key)}={System.Net.WebUtility.UrlEncode(content.Value)}");
            }
            var data = builder.ToString();
            return data.Substring(0, data.Length - 1);
        }

        protected IHttpClient CreatePostHttp()
        {
            var client = CreateHttp();
            client.Method = RequestMethod.Post;
            return client;
        }
    }
}
