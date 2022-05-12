using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Http
{

    public delegate void HttpExceptionFunc(HttpException ex);

    public class HttpException
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        public HttpException()
        {

        }

        public HttpException(string message)
        {
            Message = message;
        }

        public HttpException(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
