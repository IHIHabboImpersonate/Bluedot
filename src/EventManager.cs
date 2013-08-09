using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.ApiUsage.Plugins;

namespace Bluedot.HabboServer.Events
{
    public class EventManager
    {
        private struct EventIdentity
        {
            private string _eventName;
            private EventPriority _priority;
            
            internal EventIdentity(string eventName, EventPriority priority)
            {
                _eventName = eventName;
                _priority = priority;
            }
        }

        private readonly Dictionary<EventIdentity, WeakHashSet<EventHandler>> _weakEvents;
        private readonly Dictionary<EventIdentity, HashSet<EventHandler>> _strongEvents;

        #region Method: EventManager (Constructor)
        public EventManager()
        {
            _weakEvents = new Dictionary<EventIdentity, WeakHashSet<EventHandler>>();
            _strongEvents = new Dictionary<EventIdentity, HashSet<EventHandler>>();
        }
        #endregion


        #region Method: StrongBind
        public EventManager StrongBind(string eventName, EventPriority priority, EventHandler handler)
        {
            EventIdentity identity = new EventIdentity(eventName, priority);
            if (!_strongEvents.ContainsKey(identity))
            {
                _strongEvents.Add(identity, new HashSet<EventHandler>());
            }
            _strongEvents[identity].Add(handler);
            return this;
        }
        #endregion
        #region Method: Bind
        public EventManager Bind(string eventName, EventPriority priority, EventHandler handler)
        {
            EventIdentity identity = new EventIdentity(eventName, priority);
            if (!_weakEvents.ContainsKey(identity))
            {
                _weakEvents.Add(identity, new WeakHashSet<EventHandler>());
            }
            _weakEvents[identity].Add(handler);
            return this;
        }
        #endregion

        #region Method: Fire
        internal EventManager Fire(IPseudoPlugin plugin, string eventName, EventPriority priority, object source, EventArgs args)
        {
            EventIdentity identity = new EventIdentity(eventName, priority);

            List<Task> tasks = new List<Task>();

            if (_strongEvents.ContainsKey(identity))
            {
                foreach (EventHandler handler in _strongEvents[identity])
                {
                    EventHandler localHandler = handler;
                    tasks.Add(Task.Factory.StartNew(() => SafeFire(localHandler, source, args)));
                }
            }
            if (_weakEvents.ContainsKey(identity))
            {
                foreach (EventHandler handler in _weakEvents[identity])
                {
                    EventHandler localHandler = handler;
                    tasks.Add(Task.Factory.StartNew(() => SafeFire(localHandler, source, args)));
                }
            }

            Task.WaitAll(tasks.ToArray());
            return this;
        }

        #region Method: SafeFire
        private void SafeFire(EventHandler handler, object source, EventArgs args)
        {
            try
            {
                handler.Invoke(source, args);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        #endregion
        #endregion
    }

    public enum EventPriority
    {
        Before,
        After
    }
}