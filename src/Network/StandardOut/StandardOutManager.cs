using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;

using Nito.Async;
using Nito.Async.Sockets;

namespace Bluedot.HabboServer.Network.StandardOut
{
    public class StandardOutManager
    {
        #region Fields
        #region Field: _channels
        private Dictionary<byte, Channel> _channels;
        #endregion
        #region Field: _channelLock
        private ReaderWriterLockSlim _channelLock;
        #endregion
        #endregion

        #region Properties
        #region Property: Channels
        /// <summary>
        /// 
        /// </summary>
        public ICollection<Channel> Channels
        {
            get
            {
                return _channels.Values;
            }
        }
        #endregion

        #region Default Channels
        #region Property: DebugChannel
        /// <summary>
        /// 
        /// </summary>
        public Channel DebugChannel
        {
            get;
            private set;
        }
        #endregion
        #region Property: NoticeChannel
        /// <summary>
        /// 
        /// </summary>
        public Channel NoticeChannel
        {
            get;
            private set;
        }
        #endregion
        #region Property: ImportantChannel
        /// <summary>
        /// 
        /// </summary>
        public Channel ImportantChannel
        {
            get;
            private set;
        }
        #endregion
        #region Property: WarningChannel
        /// <summary>
        /// 
        /// </summary>
        public Channel WarningChannel
        {
            get;
            private set;
        }
        #endregion
        #region Property: ErrorChannel
        /// <summary>
        /// 
        /// </summary>
        public Channel ErrorChannel
        {
            get;
            private set;
        }
        #endregion
        #endregion
        #endregion

        #region Methods
        #region Method: StandardOutManager (Constructor)
        public StandardOutManager(ushort standardOutPort)
        {
            _channels = new Dictionary<byte, Channel>();
            _channelLock = new ReaderWriterLockSlim();

            Start(IPAddress.Any, standardOutPort);

            DebugChannel = NewChannel();
            DebugChannel.Name = "Default Debug";

            NoticeChannel = NewChannel();
            NoticeChannel.Name = "Default Notice";

            ImportantChannel = NewChannel();
            ImportantChannel.Name = "Default Important";

            WarningChannel = NewChannel();
            WarningChannel.Name = "Default Warning";

            ErrorChannel = NewChannel();
            ErrorChannel.Name = "Default Error";
        }
        #endregion

        #region Channel API Methods
        #region Method: NewChannel
        public Channel NewChannel()
        {
            _channelLock.EnterWriteLock();
            try
            {
                if (_channels.Count == byte.MaxValue)
                    return null;

                byte id = 0;
                while (_channels.ContainsKey(id))
                    id++;

                Channel channel = new Channel(id);
                _channels[id] = channel;
                return channel;
            }
            finally
            {
                _channelLock.ExitWriteLock();
            }
        }
        #endregion
        #region Method: RemoveChannel
        public void RemoveChannel(byte id)
        {
            _channelLock.EnterWriteLock();
            try
            {
                if(_channels.ContainsKey(id))
                {
                    Channel channel = _channels[id];
                    _channels.Remove(id);
                    channel.Stop();
                }
            }
            finally
            {
                _channelLock.ExitWriteLock();
            }
        }
        #endregion
        #region Method: GetChannel
        internal Channel GetChannel(byte id)
        {
            _channelLock.EnterReadLock();
            try
            {
                if (!_channels.ContainsKey(id))
                    return null;
                return _channels[id];
            }
            finally
            {
                _channelLock.ExitReadLock();
            }
        }
        #endregion
        #endregion

        #region Socket Methods
        private ServerTcpSocket _listeningSocket;
        private ActionThread _actionThread;

        public StandardOutManager Start(IPAddress bindAddress, ushort bindPort)
        {
            _actionThread = new ActionThread
            {
                Name = "BLUEDOT-StandardOutManagerThread"
            };

            _actionThread.Start();
            _actionThread.Do(() =>
            {
                _listeningSocket = new ServerTcpSocket();
                _listeningSocket.AcceptCompleted += IncomingConnectedAccepted;
                _listeningSocket.Bind(bindAddress, bindPort);
                _listeningSocket.AcceptAsync();
            });

            return this;
        }

        public StandardOutManager Stop()
        {
            foreach (Channel channel in Channels)
            {
                channel.Stop();
            }

            _listeningSocket.Close();
            _actionThread.Join();

            return this;
        }

        private void IncomingConnectedAccepted(AsyncResultEventArgs<ServerChildTcpSocket> args)
        {
            if (args.Error != null)
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
            StandardOutConnection socket = new StandardOutConnection(internalSocket);

            socket.Start();
            _listeningSocket.AcceptAsync();
        }
        #endregion
        #endregion
    }

    public class Channel
    {
        #region Field: _exited
        private bool _exited;
        #endregion
        #region Field: _bindings
        /// <summary>
        /// 
        /// </summary>
        private ICollection<StandardOutConnection> _bindings;
        #endregion

        #region Property: Id
        /// <summary>
        /// 
        /// </summary>
        internal byte Id
        {
            get;
            private set;
        }
        #endregion

        #region Property: Name
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion

        #region Property: DefaultBackgroundColour
        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor DefaultBackgroundColour
        {
            get;
            set;
        }
        #endregion
        #region Property: DefaultForegroundColour
        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor DefaultForegroundColour
        {
            get;
            set;
        }
        #endregion

        #region Method: Channel (Constructor)
        public Channel(byte id)
        {
            _bindings = new HashSet<StandardOutConnection>();

            Id = id;
            DefaultBackgroundColour = ConsoleColor.Black;
            DefaultForegroundColour = ConsoleColor.Gray;
        }
        #endregion

        #region Method: Clear
        public void Clear()
        {
            if (_exited)
                return;

            foreach (var connection in _bindings)
            {
                connection.SendClearChannelBuffer(Id);
            }
        }
        #endregion
        #region Method: WriteMessage
        public void WriteMessage(string message)
        {
            if (_exited)
                return;

            foreach (var connection in _bindings)
            {
                connection.SendWriteChannelBuffer(Id, DefaultBackgroundColour, DefaultForegroundColour, message);
            }
        }
        #endregion
        #region Method: WriteMessage
        public void WriteMessage(string message, ConsoleColor backgroundColour, ConsoleColor foregroundColour)
        {
            if (_exited)
                return;

            foreach (var connection in _bindings)
            {
                connection.SendWriteChannelBuffer(Id, backgroundColour, foregroundColour, message);
            }
        }
        #endregion

        #region Method: Bind
        internal void Bind(StandardOutConnection connection)
        {
            if (_exited)
                throw new Exception("The Channel is no longer active and cannot be bound to.");

            _bindings.Add(connection);
            connection.SendBoundToChannel(Id);
        }
        #endregion
        #region Method: Unbind
        internal void Unbind(StandardOutConnection connection)
        {
            if (_exited)
                return;

            if (_bindings.Remove(connection))
                connection.SendUnboundFromChannel(Id);
        }
        #endregion

        #region Method: Stop
        internal void Stop()
        {
            if (_exited)
                return;

            _exited = true;
            foreach (StandardOutConnection connection in _bindings)
            {
                connection.SendUnboundFromChannel(Id);
            }
            _bindings.Clear();
        }
        #endregion
    }
}
