using IHI.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHI.Server.Network.GameSockets
{
    public class GameSocketProtocol
    {
        public Version Client
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

        public GameSocketProtocol(GameSocketReader reader, int clientRelease = 0, int clientDate = 0, int clientTime = 0, int clientBuild = 0) : this(reader, null)
        {
            Client =  new Version(clientRelease, clientDate, clientTime, clientBuild);
        }
        public GameSocketProtocol(GameSocketReader reader, Version client) : this(reader, client, new GameSocketMessageHandlerInvokerManager()) { }
        public GameSocketProtocol(GameSocketReader reader, Version client, GameSocketMessageHandlerInvokerManager handlerInvokerManager)
        {
            Reader = reader;
            Client = client;
            HandlerInvokerManager = handlerInvokerManager;
        }
    }
}
