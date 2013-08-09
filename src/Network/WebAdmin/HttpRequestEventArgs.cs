using System;
using System.Net;

namespace IHI.Server.Network.WebAdmin
{
    public class HttpRequestEventArgs : EventArgs
    {
        public HttpRequestEventArgs(HttpListenerContext requestContext)
        {
            RequestContext = requestContext;
        }

        public HttpListenerContext RequestContext { get; private set; }
    }
}