using System;
using System.Net;
using Nito.Async;
using Nito.Async.Sockets;
using SmartWeakEvent;

namespace Bluedot.HabboServer.Network
{
    public delegate void GameSocketConnectionEventHandler(object source, GameSocketConnectionEventArgs args);

    public class GameSocketConnectionEventArgs : EventArgs
    {
        public GameSocket Socket
        {
            get;
            private set;
        }

        public bool Cancelled { get; private set; }

        internal GameSocketConnectionEventArgs(GameSocket socket)
        {
            Socket = socket;
        }

        public void Cancel()
        {
            Cancelled = true;
        }
    }

    public class GameSocketManager
    {
        public GameSocketReader Reader
        {
            get;
            set;
        }

        public IPAddress Address
        {
            get;
            set;
        }

        public ushort Port
        {
            get;
            set;
        }
        
        #region Event: OnPreIncomingConnection
        private readonly FastSmartWeakEvent<GameSocketConnectionEventHandler> _eventOnPreIncomingConnection = new FastSmartWeakEvent<GameSocketConnectionEventHandler>();
        /// <summary>
        /// Invoked when a connection arrives.
        /// Cancelling this event will reject the connection.
        /// </summary>
        public event GameSocketConnectionEventHandler OnPreIncomingConnection
        {
            add { _eventOnPreIncomingConnection.Add(value); }
            remove { _eventOnPreIncomingConnection.Remove(value); }
        }
        #endregion

        #region Event: OnPostIncomingConnection
        private readonly FastSmartWeakEvent<GameSocketConnectionEventHandler> _eventOnPostIncomingConnection = new FastSmartWeakEvent<GameSocketConnectionEventHandler>();
        /// <summary>
        /// Invoked when a connection has arrived, was not cancelled and is now ready.
        /// Cancelling this event has no affect.
        /// </summary>
        public event GameSocketConnectionEventHandler OnPostIncomingConnection
        {
            add { _eventOnPostIncomingConnection.Add(value); }
            remove { _eventOnPostIncomingConnection.Remove(value); }
        }
        #endregion

        private ServerTcpSocket _listeningSocket;
        private ActionThread _actionThread;

        public GameSocketManager Start()
        {
            _actionThread = new ActionThread
                                {
                                    Name = "BLUEDOT-GameSocketThread"
                                };

            _actionThread.Start();
            _actionThread.Do(() =>
            {
                _listeningSocket = new ServerTcpSocket();
                _listeningSocket.AcceptCompleted += IncomingConnectedAccepted;
                _listeningSocket.Bind(Address, Port);
                _listeningSocket.AcceptAsync();
            });

            return this;
        }

        public GameSocketManager Stop()
        {
            // TODO: Close all open connections.
            CoreManager.ServerCore.StandardOut.PrintImportant("Game Socket Manager => Stopping...");
            _listeningSocket.Close();
            _actionThread.Join();
            CoreManager.ServerCore.StandardOut.PrintImportant("Game Socket Manager => Stopped!");

            return this;
        }

        private void IncomingConnectedAccepted(AsyncResultEventArgs<ServerChildTcpSocket> args)
        {
            if(args.Error != null)
            {
                // TODO: Die safely?
                CoreManager.ServerCore.StandardOut.PrintError("Game Socket Manager => Incoming connection failed!!");
                CoreManager.ServerCore.StandardOut.PrintException(args.Error);
                _listeningSocket.AcceptAsync();
                return;
            }

            ServerChildTcpSocket internalSocket = args.Result;
            GameSocket socket = new GameSocket(internalSocket, Reader);

            GameSocketConnectionEventArgs connectionEventArgs = new GameSocketConnectionEventArgs(socket);

            if (_eventOnPreIncomingConnection != null)
            {
                _eventOnPreIncomingConnection.Raise(this, connectionEventArgs);
                if (connectionEventArgs.Cancelled)
                {
                    CoreManager.ServerCore.StandardOut.PrintNotice("Incoming connection rejected: " + internalSocket.RemoteEndPoint);
                    socket.Disconnect();
                    return;
                }
            }

            socket.Start();

            if (_eventOnPostIncomingConnection != null)
            {
                _eventOnPostIncomingConnection.Raise(this, connectionEventArgs);
                CoreManager.ServerCore.StandardOut.PrintNotice("Incoming connection accepted: " + internalSocket.RemoteEndPoint);
            }

            _listeningSocket.AcceptAsync();
        }
    }
}