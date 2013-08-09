using System;
using System.Net;

using IHI.Server.Events;
using IHI.Server.Useful;

using Nito.Async;
using Nito.Async.Sockets;
using IHI.Server.Network.GameSockets;

namespace IHI.Server.Network
{
    public class GameSocketManager
    {
        public GameSocketProtocol Protocol
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
            CoreManager.ServerCore.StandardOut.Info("Game Socket Manager => Stopping...");
            _listeningSocket.Close();
            _actionThread.Join();
            CoreManager.ServerCore.StandardOut.Info("Game Socket Manager => Stopped!");

            return this;
        }

        private void IncomingConnectedAccepted(AsyncResultEventArgs<ServerChildTcpSocket> args)
        {
            if(args.Error != null)
            {
                // TODO: Die safely?
                CoreManager.ServerCore.StandardOut.Error("Game Socket Manager => Incoming connection failed!!");

                // TODO: Pretty exception reporting
                Console.WriteLine();
                Console.WriteLine(args.Error.Message);
                Console.WriteLine(args.Error.StackTrace);

                _listeningSocket.AcceptAsync();
                return;
            }

            ServerChildTcpSocket internalSocket = args.Result;
            GameSocket socket = new GameSocket(internalSocket, Protocol);

            CancelReasonEventArgs connectionEventArgs = new CancelReasonEventArgs();

            CoreManager.ServerCore.OfficalEventFirer.Fire("incoming_game_connection", EventPriority.Before, socket, connectionEventArgs);
            if (connectionEventArgs.Cancel)
            {
                socket.Disconnect("Connection rejected from " + internalSocket.RemoteEndPoint + "(" + connectionEventArgs.CancelReason + ")");
                return;
            }

            socket.Start();

            CoreManager.ServerCore.OfficalEventFirer.Fire("incoming_game_connection", EventPriority.After, socket, connectionEventArgs);
            CoreManager.ServerCore.StandardOut.Info("Game Socket Manager => Incoming connection accepted: " + internalSocket.RemoteEndPoint);

            _listeningSocket.AcceptAsync();
        }
    }
}