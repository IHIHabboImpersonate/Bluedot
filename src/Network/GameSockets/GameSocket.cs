using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using IHI.Server.Habbos;

using Nito.Async;
using Nito.Async.Sockets;
using IHI.Server.Network.GameSockets;

namespace IHI.Server.Network
{
    public class GameSocket
    {
        #region Events
        #region Event: PacketArrived
        /// <summary>
        /// Indicates the completion of a packet read from the socket.
        /// </summary>
        private event Action<AsyncResultEventArgs<byte[]>> PacketArrived;
        #endregion
        #endregion

        #region Fields
        private readonly ServerChildTcpSocket _internalSocket;
        private int _bytesReceived;
        private readonly byte[] _lengthBuffer;
        private byte[] _dataBuffer;
        #endregion

        #region Properties
        #region Property: Habbo
        public Habbo Habbo
        {
            get;
            internal set;
        }
        #endregion
        #region Property: IPAddress
        public IPAddress IPAddress
        {
            get
            {
                try
                {
                    if (_internalSocket.RemoteEndPoint.AddressFamily == AddressFamily.InterNetwork)
                    {
                        byte[] ipv6Bytes = new byte[]
                                               {
                                                   // First 80 bits should be 0.
                                                   // Next 16 bits should be 1.
                                                   // The renaming 32 bits should be the IPv4 bits.
                                                   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 0, 0
                                               };

                        byte[] ipv4Bytes = _internalSocket.RemoteEndPoint.Address.GetAddressBytes();
                        ipv4Bytes.CopyTo(ipv6Bytes, 12);

                        return new IPAddress(ipv6Bytes);
                    }
                    return _internalSocket.RemoteEndPoint.Address;
                }
                catch (ObjectDisposedException)
                {
                    return null;
                }
            }
        }
        #endregion
        #region Property: Protocol
        public GameSocketProtocol Protocol
        {
            get;
            private set;
        }
        #endregion
        #region Property: PacketHandlers
        public GameSocketMessageHandlerInvoker PacketHandlers
        {
            get
            {
                return Protocol.HandlerInvokerManager[this];
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: GameSocket (Constructor)
        internal GameSocket(ServerChildTcpSocket socket, GameSocketProtocol protocol)
        {
            _internalSocket = socket;
            _lengthBuffer = new byte[protocol.Reader.LengthBytes];

            Habbo = HabboDistributor.GetPreLoginHabbo(this);
        }
        #endregion

        #region Method: Start
        /// <summary>
        /// Begins reading from the socket.
        /// </summary>
        internal GameSocket Start()
        {
            _internalSocket.ReadCompleted += SocketReadCompleted;
            PacketArrived += ParsePacket;

            ContinueReading();
            return this;
        }
        #endregion
        #region Method: Disconnect
        public GameSocket Disconnect(string reason = "No reason given")
        {
            if (_internalSocket != null)
                _internalSocket.Close();
            CoreManager.ServerCore.StandardOut.Notice("Game Socket Manager", CoreManager.ServerCore.StringLocale.GetString("CORE:INFO_NETWORK_CONNECTION_CLOSED", reason));

            Protocol.HandlerInvokerManager.DeregisterGameSocket(this);
            Habbo.LoggedIn = false;
            Habbo.Socket = null;
            Habbo = null;
            return this;
        }
        #endregion

        #region Method: ContinueReading
        /// <summary>
        /// Requests a read directly into the correct buffer.
        /// </summary>
        private void ContinueReading()
        {
            try
            {
                // Read into the appropriate buffer: length or data
                if (_dataBuffer != null)
                {
                    _internalSocket.ReadAsync(_dataBuffer, _bytesReceived, _dataBuffer.Length - _bytesReceived);
                }
                else
                {
                    _internalSocket.ReadAsync(_lengthBuffer, _bytesReceived, _lengthBuffer.Length - _bytesReceived);
                }
            }
            catch (ObjectDisposedException) { } // Socket closed.
        }
        #endregion
        #region Method: SocketReadCompleted
        private void SocketReadCompleted(AsyncResultEventArgs<int> args)
        {
            if (args.Error != null)
            {
                if (PacketArrived != null)
                    PacketArrived.Invoke(new AsyncResultEventArgs<byte[]>(args.Error));

                return;
            }

            _bytesReceived += args.Result;

            if (args.Result == 0)
            {
                if (PacketArrived != null)
                    PacketArrived.Invoke(new AsyncResultEventArgs<byte[]>(null as byte[]));
                return;
            }

            if (_dataBuffer == null)
            {
                if (_bytesReceived != Protocol.Reader.LengthBytes)
                {
                    ContinueReading();
                }
                else
                {
                    int length = Protocol.Reader.ParseLength(_lengthBuffer);

                    _dataBuffer = new byte[length];
                    _bytesReceived = 0;
                    ContinueReading();
                }
            }
            else
            {
                if (_bytesReceived != _dataBuffer.Length)
                {
                    ContinueReading();
                }
                else
                {
                    if (PacketArrived != null)
                        PacketArrived.Invoke(new AsyncResultEventArgs<byte[]>(_dataBuffer));

                    _dataBuffer = null;
                    _bytesReceived = 0;
                    ContinueReading();
                }
            }
        }
        #endregion
        #region Method: ParseByteData
        /// <summary>
        /// Parses a byte array as a packet.
        /// </summary>
        /// <param name="data">The byte array to parse.</param>
        public GameSocket ParseByteData(byte[] data)
        {
            IncomingMessage message = Protocol.Reader.ParseMessage(data);
#if DEBUG
            CoreManager.ServerCore.StandardOut.Debug("Packet Logging", "INCOMING => " + data.ToUtf8String());
#endif
            Task.Factory.StartNew(() => Protocol.HandlerInvokerManager[this].Invoke(Habbo, message));

            return this;
        }
        #endregion
        #region Method: ParsePacket
        private void ParsePacket(AsyncResultEventArgs<byte[]> args)
        {
            try
            {
                if (args.Error != null)
                    throw args.Error;

                if (args.Result == null)
                {
                    if (Habbo.LoggedIn)
                        Habbo.LoggedIn = false;
                    Disconnect("Socket read error!");
                    return;
                }

                ParseByteData(args.Result);
            }
            catch (Exception)
            {
                if (args.Error != null)
                {
                    CoreManager.ServerCore.StandardOut.Error("Game Socket Manager", CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_NETWORK_CONNECTION_KILLED"));
                    Console.WriteLine();
                    Console.WriteLine(args.Error.Message);
                    Console.WriteLine(args.Error.StackTrace);
                }
            }
        }
        #endregion

        #region Method: Send
        public GameSocket Send(byte[] data)
        {
            _internalSocket.WriteAsync(data);
            return this;
        }

        #endregion

        #region Method: ToString
        public override string ToString()
        {
            return _internalSocket.ToString();
        }
        #endregion
        #endregion
    }
}
