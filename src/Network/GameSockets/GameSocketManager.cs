using System;
using System.Net;

using Bluedot.HabboServer.Network.StandardOut;
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

#if DEBUG
        internal Channel PacketOutputChannel
        {
            get;
            private set;
        }
#endif

        private ServerTcpSocket _listeningSocket;
        private ActionThread _actionThread;

        public GameSocketManager Start()
        {
#if DEBUG
            PacketOutputChannel = CoreManager.ServerCore.StandardOutManager.NewChannel();
            PacketOutputChannel.Name = "Game Socket Packet Output";
#endif

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
            CoreManager.ServerCore.StandardOutManager.ImportantChannel.WriteMessage("Game Socket Manager => Stopping...");
            _listeningSocket.Close();
            _actionThread.Join();
            CoreManager.ServerCore.StandardOutManager.ImportantChannel.WriteMessage("Game Socket Manager => Stopped!");

            return this;
        }

        private void IncomingConnectedAccepted(AsyncResultEventArgs<ServerChildTcpSocket> args)
        {
            if(args.Error != null)
            {
                // TODO: Die safely?
                CoreManager.ServerCore.StandardOutManager.ErrorChannel.WriteMessage("Game Socket Manager => Incoming connection failed!!");

                // TODO: Pretty exception reporting
                Console.WriteLine();
                Console.WriteLine(args.Error.Message);
                Console.WriteLine(args.Error.StackTrace);

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
            CoreManager.ServerCore.StandardOutManager.NoticeChannel.WriteMessage("Game Socket Manager => Incoming connection accepted: " + internalSocket.RemoteEndPoint);

            _listeningSocket.AcceptAsync();
        }
    }
}