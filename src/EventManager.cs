using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Events
{
    public class EventManager
    {
        private readonly Dictionary<string, WeakHashSet<EventHandler>> _weakEvents;
        private readonly Dictionary<string, HashSet<EventHandler>> _events;

        #region Method: EventManager (Constructor)
        public EventManager()
        {
            _weakEvents = new Dictionary<string, WeakHashSet<EventHandler>>();
            _events = new Dictionary<string, HashSet<EventHandler>>();
        }
        #endregion

        #region Method: Bind
        public EventManager Bind(string eventName, EventHandler handler)
        {
            if (!_events.ContainsKey(eventName))
            {
                _events.Add(eventName, new HashSet<EventHandler>());
            }
            _events[eventName].Add(handler);
            return this;
        }
        #endregion
        #region Method: WeakBind
        public EventManager WeakBind(string eventName, EventHandler handler)
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
            if (_events.ContainsKey(eventName))
            {
                foreach (EventHandler handler in _events[eventName])
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
