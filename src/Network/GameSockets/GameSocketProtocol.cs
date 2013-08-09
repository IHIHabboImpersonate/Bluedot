using Bluedot.HabboServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluedot.HabboServer.Network.GameSockets
{
    public class GameSocketProtocol
    {
        public Version Version // They are not called versions anymore but this is the name of the type so it is clearer.
        {
            get;
            private set;
        }
        public GameSocketReader Reader
        {
            get;
            private set;
        }   

        public GameSocketMessageHandlerInvokerManager HandlerInvokerManager
        {
            get;
            private set;
        }

        public GameSocketProtocol(Version version, GameSocketReader reader) : this(version, reader, new GameSocketMessageHandlerInvokerManager()) { }
        public GameSocketProtocol(Version version, GameSocketReader reader, GameSocketMessageHandlerInvokerManager handlerInvokerManager)
        {
            Reader = reader;
            HandlerInvokerManager = handlerInvokerManager;
        }
    }
}
