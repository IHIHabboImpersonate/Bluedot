#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Globalization;
using System.Net;
using System.Threading;

#endregion

namespace Bluedot.HabboServer.Network.WebAdmin
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
                throw new NotSupportedException("The HttpListener class is not supported on this operating system.");
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
                if (e.Message == "The process cannot access the file because it is being used by another process")
                    throw new Exception("The WebAdminServer was unable to start. Is the port already in use?", e);
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
            catch
            {
                return;
            }
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
                                                                     "ConnectionManager_{0}",
                                                                     UniqueId)
                                               };
            }
            else if (_connectionManagerThread.ThreadState == ThreadState.Running)
            {
                throw new ThreadStateException("The request handling process is already running.");
            }

            if (_connectionManagerThread.ThreadState != ThreadState.Unstarted)
            {
                throw new ThreadStateException(
                    "The request handling process was not properly initialized so it could not be started.");
            }
            _connectionManagerThread.Start();

            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond*10;
            while (RunState != State.Started)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime)
                {
                    throw new TimeoutException("Unable to start the request handling process.");
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
                    throw new TimeoutException("Unable to stop the web server process.");
                }
            }

            _connectionManagerThread = null;
        }
        #endregion
        #endregion
    }
}