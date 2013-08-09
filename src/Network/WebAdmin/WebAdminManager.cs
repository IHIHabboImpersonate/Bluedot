#region Usings

using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace IHI.Server.Network.WebAdmin
{
    public class WebAdminManager
    {
        #region Fields
        #region Field: _paths
        private readonly Dictionary<string, HttpPathHandler> _paths;
        #endregion
        #region Field: _port
        private readonly ushort _port;
        #endregion
        #region Field: _stopWaiter
        private readonly AutoResetEvent _stopWaiter;
        #endregion
        #endregion

        #region Indexers
        #region Indexer: string
        public HttpPathHandler this[string path]
        {
            get
            {
                lock (_paths)
                {
                    if (!_paths.ContainsKey(path))
                        return null;

                    return _paths[path];
                }
            }
            set
            {
                lock (_paths)
                {
                    bool newEntry = true;
                    if (_paths.ContainsKey(path) && _paths[path] != value)
                    {
                        newEntry = false;
                        _paths.Remove(path);
                    }

                    if (value == null)
                    {
                        CoreManager.ServerCore.StandardOut.Debug("Web Admin => " + CoreManager.ServerCore.StringLocale.GetString("CORE:DEBUG_WEBADMIN_HANDLER_REMOVED",  path));
                        return;
                    }
                    _paths.Add(path, value);
                    if(newEntry)
                        CoreManager.ServerCore.StandardOut.Debug("Web Admin => " + CoreManager.ServerCore.StringLocale.GetString("CORE:DEBUG_WEBADMIN_HANDLER_ADDED", path));
                    else
                        CoreManager.ServerCore.StandardOut.Debug("Web Admin => " + CoreManager.ServerCore.StringLocale.GetString("CORE:DEBUG_WEBADMIN_HANDLER_CHANGED", path));
                }
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: WebAdminManager (Constructor)
        internal WebAdminManager(ushort port)
        {
            _paths = new Dictionary<string, HttpPathHandler>();
            _port = port;
            _stopWaiter = new AutoResetEvent(false);

            new Thread(Run).Start();
        }
        #endregion

        #region Method: Run
        /// <summary>
        ///   Ensures the web server is running.
        /// </summary>
        private void Run()
        {
            using (WebAdminServer listener = new WebAdminServer(_port))
            {
                listener.IncomingRequest += Handle;
                listener.Start();

                _stopWaiter.WaitOne();
            }
        }
        #endregion

        #region Method: Handle
        private void Handle(object sender, HttpRequestEventArgs e)
        {
            string path = e.RequestContext.Request.Url.AbsolutePath;
            lock (_paths)
            {
                HttpPathHandler handler = this[path];
                if (handler != null)
                {
                    CoreManager.ServerCore.StandardOut.Debug("Web Admin => WebAdmin Request [200]: " + path);
                    Task.Factory.StartNew(() => handler(e.RequestContext));
                    return;
                }
            }
            CoreManager.ServerCore.StandardOut.Debug("Web Admin => WebAdmin Request [404]: " + path);

            HttpListenerResponse response = e.RequestContext.Response;
            byte[] buffer = Encoding.UTF8.GetBytes("Not Handled!");
            response.StatusCode = (int) HttpStatusCode.NotFound;
            response.StatusDescription = "Not Found";
            response.ContentLength64 = buffer.Length;
            response.ContentEncoding = Encoding.UTF8;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            response.Close();
        }
        #endregion

        #region Method: Stop
        /// <summary>
        ///   Stops the web server.
        /// </summary>
        internal void Stop()
        {
            _stopWaiter.Set();
        }
        #endregion

        #region Method: SendResponse
        public static void SendResponse(HttpListenerResponse response, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.StatusCode = (int) HttpStatusCode.OK;
            response.StatusDescription = "OK";
            response.ContentType = "text/html; charset=UTF-8";
            response.ContentLength64 = buffer.Length;
            response.ContentEncoding = Encoding.UTF8;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            response.Close();
        }
        #endregion
        #endregion
    }
}