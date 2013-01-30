using System;
using System.Collections.Generic;
using System.Text;

using Nito.Async;
using Nito.Async.Sockets;

namespace Bluedot.HabboServer.Network.StandardOut
{
    internal class StandardOutConnection
    {
        #region Constants
        private const byte ProtocolVersion = 1;
        #endregion

        #region Events
        #region Event: PacketArrived
        /// <summary>
        /// Indicates the completion of a packet read from the socket.
        /// </summary>
        private event Action<AsyncResultEventArgs<byte[]>> PacketArrived;
        #endregion
        #endregion

        #region Fields
        #region Field: _internalSocket
        private ServerChildTcpSocket _internalSocket;
        #endregion
        #region Field: _bytesReceived
        private int _bytesReceived;
        #endregion
        #region Field: _lengthBuffer
        private readonly byte[] _lengthBuffer;
        #endregion
        #region Field: _dataBuffer
        private byte[] _dataBuffer;
        #endregion
        #region Field: _handlers
        private Action<byte[]>[] _handlers;
        #endregion
        #endregion

        #region Methods
        #region Method: StandardOutConnection (Constructor)
        public StandardOutConnection(ServerChildTcpSocket internalSocket)
        {
            _internalSocket = internalSocket;
            _lengthBuffer = new byte[4];
            _handlers = new Action<byte[]>[255];
            
            _handlers[0x00] = ProcessRequestProtocolVersion;
            _handlers[0x01] = ProcessRequestEncryption;
            _handlers[0x10] = ProcessRequestChannelList;
            _handlers[0x20] = ProcessRequestChannelUnbind;
            _handlers[0x21] = ProcessRequestChannelBind;
        }
        #endregion
        
        #region Network Methods
        #region Socket Methods
        #region Method: Start
        public StandardOutConnection Start()
        {
            Console.WriteLine("New StandardOut Connection (" + _internalSocket.RemoteEndPoint.Address + ":" + _internalSocket.RemoteEndPoint.Port + ")");
            _internalSocket.ReadCompleted += SocketReadCompleted;
            PacketArrived += ParsePacket;

            ContinueReading();
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
        #region Method: SendRaw
        private void SendRaw(byte header, byte[] data)
        {
            byte[] fullData = new byte[data.Length + 5];
            int length = data.Length + 1;
            fullData[0] = (byte)(length >> 030);
            fullData[1] = (byte)(length >> 020);
            fullData[2] = (byte)(length >> 010);
            fullData[3] = (byte)length;
            fullData[4] = header;
            data.CopyTo(fullData, 5);

            _internalSocket.WriteAsync(fullData);
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
                if (_bytesReceived != _lengthBuffer.Length)
                {
                    ContinueReading();
                }
                else
                {
                    int length = _lengthBuffer[0];
                    length <<= 010;
                    length |= _lengthBuffer[1];
                    length <<= 010;
                    length |= _lengthBuffer[2];
                    length <<= 010;
                    length |= _lengthBuffer[3];

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
        #region Method: ParsePacket
        private void ParsePacket(AsyncResultEventArgs<byte[]> args)
        {
            try
            {
                if (args.Error != null)
                    throw args.Error;

                if (args.Result == null)
                {
                    Disconnect();
                    return;
                }

                byte header = args.Result[0];
                byte[] body = new byte[args.Result.Length-1];
                Array.Copy(args.Result, 1, body, 0, body.Length);

                _handlers[header](body);
            }
            catch (Exception)
            {
                if (args.Error != null)
                {
                    Console.WriteLine("StandardOut Connection Killed: ");
                    // TODO: Pretty exception reporting
                    Console.WriteLine(args.Error.Message);
                    Console.WriteLine(args.Error.StackTrace);
                }
            }
        }
        #endregion
        #region Method: Disconnect
        public StandardOutConnection Disconnect()
        {
            if (_internalSocket != null)
            {
                Console.WriteLine("StandardOut Connection Closed (" + _internalSocket.RemoteEndPoint.Address + ":" + _internalSocket.RemoteEndPoint.Port + ")");
                _internalSocket.Close();
            }
            Console.WriteLine("StandardOut Connection Closed");
            return this;
        }
        #endregion
        #endregion
        #region Protocol Methods
        #region Incoming
        /// <summary>
        /// Header: 0x00
        /// </summary>
        private void ProcessRequestProtocolVersion(byte[] data)
        {
            byte requestedVersion = data[0];
            SendUseProtocolVersion(ProtocolVersion);
        }

        /// <summary>
        /// Header: 0x01
        /// </summary>
        private void ProcessRequestEncryption(byte[] data)
        {
            // TODO: Add encryption
            SendUseEncryption(StandardOutEncryptionMethod.Off);
            return;
            StandardOutEncryptionMethod requestedMethod = (StandardOutEncryptionMethod)data[0];

            switch (requestedMethod)
            {
                case StandardOutEncryptionMethod.Off:
                    {
                        SendUseEncryption(StandardOutEncryptionMethod.Off);
                        break;
                    }
                case StandardOutEncryptionMethod.Version1:
                    {
                        SendUseEncryption(StandardOutEncryptionMethod.Version1);
                        break;
                    }
                default:
                    {
                        SendUseEncryption(StandardOutEncryptionMethod.Version1);
                        break;
                    }
            }
        }

        /// <summary>
        /// Header: 0x10
        /// </summary>
        private void ProcessRequestChannelList(byte[] data)
        {
            Dictionary<byte, string> channelData = new Dictionary<byte, string>();

            foreach (Channel channel in CoreManager.ServerCore.StandardOutManager.Channels)
            {
                channelData.Add(channel.Id, channel.Name);
            }

            SendChannelListUpdate(channelData);
        }

        /// <summary>
        /// Header: 0x20
        /// </summary>
        private void ProcessRequestChannelBind(byte[] data)
        {
            byte channelId = data[0];
            Channel channel = CoreManager.ServerCore.StandardOutManager.GetChannel(channelId);
            if (channel != null)
                channel.Bind(this);
        }

        /// <summary>
        /// Header: 0x21
        /// </summary>
        private void ProcessRequestChannelUnbind(byte[] data)
        {
            byte channelId = data[0];
            Channel channel = CoreManager.ServerCore.StandardOutManager.GetChannel(channelId);
            if (channel != null)
                channel.Unbind(this);
        }
        #endregion

        #region Outgoing
        /// <summary>
        /// Header: 0x00
        /// </summary>
        internal void SendUseProtocolVersion(byte protocolVersion)
        {
            byte[] data = new[] { protocolVersion };

            SendRaw(0x00, data);
        }

        /// <summary>
        /// Header: 0x01
        /// </summary>
        internal void SendUseEncryption(StandardOutEncryptionMethod encryptionMethod)
        {
            byte[] data = new[] { (byte)encryptionMethod };

            SendRaw(0x01, data);
        }

        /// <summary>
        /// Header: 0x10
        /// </summary>
        internal void SendChannelListUpdate(IDictionary<byte, string> channels)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)channels.Count);
            foreach (KeyValuePair<byte, string> channel in channels)
            {
                data.Add(channel.Key);
                data.Add((byte)(channel.Value.Length >> 010));
                data.Add((byte)channel.Value.Length);
                data.AddRange(Encoding.UTF8.GetBytes(channel.Value));
            }

            SendRaw(0x10, data.ToArray());
        }

        /// <summary>
        /// Header: 0x20
        /// </summary>
        internal void SendBoundToChannel(byte id)
        {
            byte[] data = new[] { id };

            SendRaw(0x30, data);
        }

        /// <summary>
        /// Header: 0x21
        /// </summary>
        internal void SendUnboundFromChannel(byte id)
        {
            byte[] data = new[] { id };

            SendRaw(0x21, data);
        }

        /// <summary>
        /// Header: 0x30
        /// </summary>
        internal void SendClearChannelBuffer(byte id)
        {
            byte[] data = new[] { id };

            SendRaw(0x30, data);
        }

        /// <summary>
        /// Header: 0x31
        /// </summary>
        internal void SendWriteChannelBuffer(byte channelId, ConsoleColor backgroundColour, ConsoleColor foregroundColour, string message)
        {
            List<byte> data = new List<byte>();
            data.Add(channelId);
            data.Add((byte)backgroundColour);
            data.Add((byte)foregroundColour);
            data.Add((byte)(message.Length >> 010));
            data.Add((byte)message.Length);
            data.AddRange(Encoding.UTF8.GetBytes(message));

            SendRaw(0x31, data.ToArray());
        }
        #endregion
        #endregion
        #endregion
        #endregion
    }
}