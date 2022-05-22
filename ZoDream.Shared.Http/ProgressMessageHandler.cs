using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Http
{
    public class ProgressMessageHandler: MessageProcessingHandler
    {

        /// <summary>
        /// Occurs every time the client sending data is making progress.
        /// </summary>
        public event HttpProgressEventHandler? HttpSendProgress;

        /// <summary>
        /// Occurs every time the client receiving data is making progress.
        /// </summary>
        public event HttpProgressEventHandler? HttpReceiveProgress;

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (HttpSendProgress == null)
            {
                return request;
            }
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (HttpReceiveProgress == null)
            {
                return response;
            }
            return response;
        }
    }

    public delegate void HttpProgressEventHandler(long progress, long total);
}
