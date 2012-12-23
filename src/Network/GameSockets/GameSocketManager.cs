using System;
using System.Net;
using Nito.Async;
using Nito.Async.Sockets;

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
            CoreManager.ServerCore.StandardOut.PrintImportant("Game Socket Manager", "Stopping...");
            _listeningSocket.Close();
            _actionThread.Join();
            CoreManager.ServerCore.StandardOut.PrintImportant("Game Socket Manager", "Stopped!");

            return this;
        }

        private void IncomingConnectedAccepted(AsyncResultEventArgs<ServerChildTcpSocket> args)
        {
            if(args.Error != null)
            {
                // TODO: Die safely?
                CoreManager.ServerCore.StandardOut.PrintError("Game Socket Manager", "Incoming connection failed!!");
                CoreManager.ServerCore.StandardOut.PrintException(args.Error);
                _listeningSocket.AcceptAsync();
                return;
            }

            ServerChildTcpSocket internalSocket = args.Result;
            GameSocket socket = new GameSocket(internalSocket, Reader);

            GameSocketConnectionEventArgs connectionEventArgs = new GameSocketConnectionEventArgs(socket);

            CoreManager.ServerCore.EventManager.Fire("incoming_game_connection:before", this, connectionEventArgs);
            if (connectionEventArgs.Cancelled)
            {
                CoreManager.ServerCore.StandardOut.PrintNotice("Game Socket Manager", "Incoming connection rejected: " + internalSocket.RemoteEndPoint);
                socket.Disconnect();
                return;
            }

            socket.Start();
            CoreManager.ServerCore.EventManager.Fire("incoming_game_connection:after", this, connectionEventArgs);
            CoreManager.ServerCore.StandardOut.PrintNotice("Game Socket Manager", "Incoming connection accepted: " + internalSocket.RemoteEndPoint);

            _listeningSocket.AcceptAsync();
        }
    }
}