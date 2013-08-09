using System;
using System.Collections.Generic;
using IHI.Server.Habbos;

namespace IHI.Server.Network
{
    public class GameSocketMessageHandlerInvokerManager
    {
        private readonly Dictionary<GameSocket, GameSocketMessageHandlerInvoker> _invokers;

        public GameSocketMessageHandlerInvokerManager()
        {
            _invokers = new Dictionary<GameSocket, GameSocketMessageHandlerInvoker>();
        }

        public GameSocketMessageHandlerInvoker this[GameSocket gameSocket]
        {
            get
            {
                lock (_invokers)
                {
                    if (!_invokers.ContainsKey(gameSocket))
                        return null;
                    return _invokers[gameSocket];
                }
            }
            set
            {
                lock (_invokers)
                {
                    if (!_invokers.ContainsKey(gameSocket))
                        return;
                    _invokers.Add(gameSocket, value);
                }
            }
        }

        public void DeregisterGameSocket(GameSocket gameSocket)
        {
            _invokers.Remove(gameSocket);
        }
    }
}