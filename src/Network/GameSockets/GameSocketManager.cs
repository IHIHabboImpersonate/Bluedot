using System;
using System.Net;

using Bluedot.HabboServer.Useful;

using Nito.Async;
using Nito.Async.Sockets;

namespace Bluedot.HabboServer.Network
{
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

            CancelReasonEventArgs connectionEventArgs = new CancelReasonEventArgs();

            CoreManager.ServerCore.EventManager.Fire("incoming_game_connection:before", socket, connectionEventArgs);
            if (connectionEventArgs.Cancel)
            {
                socket.Disconnect("Connection rejected from " + internalSocket.RemoteEndPoint + "(" + connectionEventArgs.CancelReason + ")");
                return;
            }

            socket.Start();
            CoreManager.ServerCore.EventManager.Fire("incoming_game_connection:after", socket, connectionEventArgs);
            CoreManager.ServerCore.StandardOut.PrintNotice("Game Socket Manager", "Incoming connection accepted: " + internalSocket.RemoteEndPoint);

            _listeningSocket.AcceptAsync();
        }
    }
}