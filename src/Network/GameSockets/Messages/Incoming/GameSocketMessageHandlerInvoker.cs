using System.Collections.Generic;
using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.Network
{
    public class GameSocketMessageHandlerInvoker
    {
        private readonly Dictionary<int, GameSocketMessageHandlers> _handlers;

        public GameSocketMessageHandlerInvoker()
        {
            _handlers = new Dictionary<int, GameSocketMessageHandlers>();
        }

        public GameSocketMessageHandlerInvoker Invoke(Habbo sender, IncomingMessage message)
        {
            lock (_handlers)
            {
                // Are there any handlers registered for this packet?
                if (!_handlers.ContainsKey(message.HeaderId))
                {
                    CoreManager.ServerCore.StandardOutManager.DebugChannel.WriteMessage("Game Socket Manager => Unhandled HeaderID " + message.HeaderId + " (\"" + message.HeaderString + "\")");
                    // No, do nothing.
                    return this;
                }

                // Yes, let's invoke them.
                _handlers[message.HeaderId].Invoke(sender, message);
            }

            return this;
        }

        public GameSocketMessageHandler this[int headerId, GameSocketMessageHandlerPriority priority]
        {
            get
            {
                lock (_handlers)
                {
                    if (!_handlers.ContainsKey(headerId))
                        return null;
                    switch (priority)
                    {
                        case GameSocketMessageHandlerPriority.HighPriority:
                            {
                                return _handlers[headerId].HighPriority;
                            }
                        case GameSocketMessageHandlerPriority.LowPriority:
                            {
                                return _handlers[headerId].LowPriority;
                            }
                        case GameSocketMessageHandlerPriority.DefaultAction:
                            {
                                return _handlers[headerId].DefaultAction;
                            }
                        case GameSocketMessageHandlerPriority.Watcher:
                            {
                                return _handlers[headerId].Watcher;
                            }
                        default:
                            {
                                return null;
                            }
                    }
                }
            }
            set
            {
                lock (_handlers)
                {
                    if (!_handlers.ContainsKey(headerId))
                        _handlers.Add(headerId, new GameSocketMessageHandlers());

                    switch (priority)
                    {
                        case GameSocketMessageHandlerPriority.HighPriority:
                            {
                                _handlers[headerId].HighPriority = value;
                                break;
                            }
                        case GameSocketMessageHandlerPriority.LowPriority:
                            {
                                _handlers[headerId].LowPriority = value;
                                break;
                            }
                        case GameSocketMessageHandlerPriority.DefaultAction:
                            {
                                _handlers[headerId].DefaultAction = value;
                                break;
                            }
                        case GameSocketMessageHandlerPriority.Watcher:
                            {
                                _handlers[headerId].Watcher = value;
                                break;
                            }
                    }

                    if (value != null) 
                        return;

                    if (_handlers[headerId].HighPriority == null &&
                        _handlers[headerId].LowPriority == null &&
                        _handlers[headerId].DefaultAction == null &&
                        _handlers[headerId].Watcher == null)
                    {
                        _handlers.Remove(headerId);
                    }
                }
            }
        }
    }
}