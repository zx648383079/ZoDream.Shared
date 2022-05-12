using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ZoDream.Shared.Http
{
    public class JsonStringContent
    {

        public object? Content { get; set; }

        /// <summary>
        /// Creates <see cref="StringContent"/> formatted as UTF8 application/json.
        /// </summary>
        public JsonStringContent(object? obj)
        {
            Content = obj;
        }

        public StringContent ToHttpContent()
        {
            return ParseJson(JsonConvert.SerializeObject(Content));
        }

        public static StringContent ParseJson(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
