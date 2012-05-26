using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Bluedot.HabboServer.Habbos;
using Nito.Async;
using Nito.Async.Sockets;
using SmartWeakEvent;

namespace Bluedot.HabboServer.Network
{
    public class GameSocket
    {
        #region Event: PacketArrived
        /// <summary>
        /// Indicates the completion of a packet read from the socket.
        /// </summary>
        public event Action<AsyncResultEventArgs<byte[]>> PacketArrived;
        #endregion

        public GameSocketMessageHandlerInvoker PacketHandlers
        {
            get;
            set;
        }

        public Habbo Habbo
        {
            get;
            internal set;
        }

        public IPAddress IPAddress
        {
            get
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
        }

        internal GameSocket(ServerChildTcpSocket socket, GameSocketReader protocolReader)
        {
            _internalSocket = socket;
            _protocolReader = protocolReader;
            _lengthBuffer = new byte[_protocolReader.LengthBytes];
            PacketHandlers = new GameSocketMessageHandlerInvoker();

            Habbo = CoreManager.ServerCore.HabboDistributor.GetPreLoginHabbo(this);
        }

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

        /// <summary>
        /// Parses a byte array as a packet.
        /// </summary>
        /// <param name="data">The byte array to parse.</param>
        public GameSocket ParseByteData(byte[] data)
        {
            IncomingMessage message = _protocolReader.ParseMessage(data);
            PacketHandlers.Invoke(Habbo, message);

            return this;
        }

        #region Behind The Scenes Socket Stuff
        private ServerChildTcpSocket _internalSocket;
        private int _bytesReceived;
        private byte[] _lengthBuffer;
        private byte[] _dataBuffer;
        private GameSocketReader _protocolReader;

        private void SocketReadCompleted(AsyncResultEventArgs<int> args)
        {
            if(args.Error != null)
            {
                if(PacketArrived != null)
                    PacketArrived.Invoke(new AsyncResultEventArgs<byte[]>(args.Error));

                return;
            }

            _bytesReceived += args.Result;

            if(args.Result == 0)
            {
                if (PacketArrived != null)
                    PacketArrived.Invoke(new AsyncResultEventArgs<byte[]>(null as byte[]));
                return;
            }

            if(_dataBuffer == null)
            {
                if(_bytesReceived != _protocolReader.LengthBytes)
                {
                    ContinueReading();
                }
                else
                {
                    int length = _protocolReader.ParseLength(_lengthBuffer);

                    _dataBuffer = new byte[length];
                    _bytesReceived = 0;
                    ContinueReading();
                }
            }
            else
            {
                if(_bytesReceived != _dataBuffer.Length)
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

        /// <summary>
        /// Requests a read directly into the correct buffer.
        /// </summary>
        private void ContinueReading()
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

        private void ParsePacket(AsyncResultEventArgs<byte[]> args)
        {
            try
            {
                if(args.Error != null)
                    throw args.Error;

                if(args.Result == null)
                {
                    CoreManager.ServerCore.StandardOut.PrintNotice("Client Connection Closed: Gracefully close.");
                    Close();
                    return;
                }

                ParseByteData(args.Result);
            }
            catch (Exception)
            {
                if (args.Error != null)
                {
                    CoreManager.ServerCore.StandardOut.PrintError("Client Connection Killed: Socket read error!");
                    CoreManager.ServerCore.StandardOut.PrintException(args.Error);
                }
            }

            return;
        }
        
        public GameSocket Close()
        {
            if (_internalSocket != null)
                _internalSocket.Close();

            return this;
        }
        #endregion

        public GameSocket Send(byte[] data)
        {
            _internalSocket.WriteAsync(data);
            return this;
        }

        public override string ToString()
        {
            return _internalSocket.ToString();
        }
    }
}
