using System.Collections.Generic;

namespace Bluedot.HabboServer.Habbos
{
    public class SubscriptionCollection
    {
        private Dictionary<string, SubscriptionData> _subscriptionData;

        #region Property: Subscriber
        /// <summary>
        /// 
        /// </summary>
        public Habbo Subscriber
        {
            get;
            private set;
        }
        #endregion

        public SubscriptionCollection(Habbo subscriber)
        {
            Subscriber = subscriber;
            _subscriptionData = new Dictionary<string, SubscriptionData>();
        }

        public SubscriptionData this[string subscriptionName]
        {
            get
            {
                lock (_subscriptionData)
                {
                    if (!_subscriptionData.ContainsKey(subscriptionName))
                        _subscriptionData[subscriptionName] = new SubscriptionData(Subscriber, subscriptionName).Reload();
                    return _subscriptionData[subscriptionName];
                }
            }
            set
            {
                lock (_subscriptionData)
                {
                    if (value == null && _subscriptionData.ContainsKey(subscriptionName))
                        _subscriptionData.Remove(subscriptionName);
                }
            }
        }
    }
}