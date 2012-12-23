using System;
using System.Collections.Generic;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Events
{
    public class EventManager
    {
        private readonly Dictionary<string, WeakHashSet<EventHandler>> _weakEvents;
        private readonly Dictionary<string, HashSet<EventHandler>> _strongEvents;

        #region Method: EventManager (Constructor)
        public EventManager()
        {
            _weakEvents = new Dictionary<string, WeakHashSet<EventHandler>>();
            _strongEvents = new Dictionary<string, HashSet<EventHandler>>();
        }
        #endregion


        #region Method: StrongBind
        public EventManager StrongBind(string eventName, EventHandler handler)
        {
            if (!_strongEvents.ContainsKey(eventName))
            {
                _strongEvents.Add(eventName, new HashSet<EventHandler>());
            }
            _strongEvents[eventName].Add(handler);
            return this;
        }
        #endregion
        #region Method: Bind
        public EventManager Bind(string eventName, EventHandler handler)
        {
            if (!_weakEvents.ContainsKey(eventName))
            {
                _weakEvents.Add(eventName, new WeakHashSet<EventHandler>());
            }
            _weakEvents[eventName].Add(handler);
            return this;
        }
        #endregion

        #region Method: Fire
        public EventManager Fire(string eventName, object source, EventArgs args)
        {
            if (_strongEvents.ContainsKey(eventName))
            {
                foreach (EventHandler handler in _strongEvents[eventName])
                {
                    handler.Invoke(source, args);
                }
            }
            if (_weakEvents.ContainsKey(eventName))
            {
                foreach (EventHandler handler in _weakEvents[eventName])
                {
                    handler.Invoke(source, args);
                }
            }
            return this;
        }
        #endregion
    }
}
