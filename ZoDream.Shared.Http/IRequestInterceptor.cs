using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Http
{
    public interface IRequestInterceptor
    {
        IHttpClient Request(IHttpClient client);

        T Response<T>(string content);

        HttpException ResponseFailure(HttpException ex);
    }
}
