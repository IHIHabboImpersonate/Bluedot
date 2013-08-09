#region Usings

using System;
using System.Globalization;
using System.Net;
using System.Threading;

#endregion

namespace IHI.Server.Network.WebAdmin
{
    internal sealed class WebAdminServer : IDisposable
    {
        #region SubTypes
        #region SubType: State
        public enum State
        {
            Stopped,
            Stopping,
            Starting,
            Started
        }
        #endregion
        #endregion

        #region Events
        #region Event: IncomingRequest
        internal event EventHandler<HttpRequestEventArgs> IncomingRequest = null;
        #endregion
        #endregion

        #region Properties
        #region Property: RunState
        private State RunState
        {
            get { return (State)Interlocked.Read(ref _runState); }
        }
        #endregion

        #region Property: UniqueId
        private Guid UniqueId { get; set; }
        #endregion
        #endregion

        #region Fields
        #region Field: _listener
        private readonly HttpListener _listener;
        #endregion

        #region Field: _connectionManagerThread
        private Thread _connectionManagerThread;
        #endregion
        #region Field: _disposed
        private bool _disposed;
        #endregion
        #region Field: _runState
        private long _runState = (long) State.Stopped;
        #endregion
        #endregion

        #region Methods
        #region Method: WebAdminServer (Constructor)
        internal WebAdminServer(ushort port)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_WEBADMIN_OS_NOT_SUPPORTED"));
            }
            UniqueId = Guid.NewGuid();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:" + port + "/");
        }
        #endregion

        #region Method: WebAdminServer (Destructor)
        ~WebAdminServer()
        {
            Dispose(false);
        }
        #endregion

        #region Method: Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (RunState != State.Stopped)
                {
                    Stop();
                }
                if (_connectionManagerThread != null)
                {
                    _connectionManagerThread.Abort();
                    _connectionManagerThread = null;
                }
            }
            _disposed = true;
        }
        #endregion

        #region Method: ConnectionManagerThreadStart
        private void ConnectionManagerThreadStart()
        {
            Interlocked.Exchange(ref _runState, (long) State.Starting);
            try
            {
                if (!_listener.IsListening)
                {
                    _listener.Start();
                }
                if (_listener.IsListening)
                {
                    Interlocked.Exchange(ref _runState, (long) State.Started);
                }

                try
                {
                    while (RunState == State.Started)
                    {
                        HttpListenerContext context = _listener.GetContext();
                        RaiseIncomingRequest(context);
                    }
                }
                catch (HttpListenerException)
                {
                    // This will occur when the listener gets shut down.
                    // Just swallow it and move on.
                }
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 32)
                    throw new Exception(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_WEBADMIN_PORT_CONFLICT"), e);
            }
            finally
            {
                Interlocked.Exchange(ref _runState, (long) State.Stopped);
            }
        }
        #endregion
        
        #region Method: RaiseIncomingRequest
        private void RaiseIncomingRequest(HttpListenerContext context)
        {
            HttpRequestEventArgs e = new HttpRequestEventArgs(context);
            try
            {
                if (IncomingRequest != null)
                {
                    IncomingRequest.BeginInvoke(this, e, null, null);
                }
            }
            catch {}
        }
        #endregion

        #region Method: Start
        public void Start()
        {
            if (_connectionManagerThread == null || _connectionManagerThread.ThreadState == ThreadState.Stopped)
            {
                _connectionManagerThread = new Thread(ConnectionManagerThreadStart)
                                               {
                                                   Name =
                                                       String.Format(CultureInfo.InvariantCulture,
                                                                     "BLUEDOT-WebAdminConnectionManager_{0}",
                                                                     UniqueId)
                                               };
            }
            else if (_connectionManagerThread.ThreadState == ThreadState.Running)
            {
                return;
                //throw new ThreadStateException("The request handling process is already running.");
            }

            if (_connectionManagerThread.ThreadState != ThreadState.Unstarted)
            {
                throw new ThreadStateException(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_WEBADMIN_INIT_FAILED"));
            }
            _connectionManagerThread.Start();

            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond*10;
            while (RunState != State.Started)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime)
                {
                    throw new TimeoutException(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_WEBADMIN_START_FAILED"));
                }
            }
        }
        #endregion

        #region Method: Stop
        private void Stop()
        {
            // Setting the runstate to something other than "started" and
            // stopping the listener should abort the AddIncomingRequestToQueue
            // method and allow the ConnectionManagerThreadStart sequence to
            // end, which sets the RunState to Stopped.
            Interlocked.Exchange(ref _runState, (long) State.Stopping);
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond*10;
            while (RunState != State.Stopped)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime)
                {
                    throw new TimeoutException(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_WEBADMIN_STOP_FAILED"));
                }
            }

            _connectionManagerThread = null;
        }
        #endregion
        #endregion
    }
}