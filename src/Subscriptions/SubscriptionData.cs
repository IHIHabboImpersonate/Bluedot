#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Data.Objects;
using System.Linq;
using Bluedot.HabboServer;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Habbos;

#endregion

namespace Bluedot.HabboServer.Habbos
{
    public class SubscriptionData
    {
        #region Fields
        #region Field: _subscriptionDatabase
        private DBSubscription _subscriptionDatabase;
        #endregion
        #endregion

        #region Properties

        #region Property: Subscriber
        /// <summary>
        /// 
        /// </summary>
        public Bluedot.HabboServer.Habbos.Habbo Subscriber
        {
            get;
            private set;
        }
        #endregion

        #region Property: Type
        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get;
            private set;
        }
        #endregion
        
        #region Property: Remaining
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Remaining
        {
            get
            {
                if (_subscriptionDatabase.LengthSkipped == 0)
                    return new TimeSpan(0); // Not started yet.

                return new TimeSpan(0, 0,
                                    DateTime.Now.GetUnixTimestamp() - _subscriptionDatabase.TotalBought + _subscriptionDatabase.LengthSkipped);
            }
            set
            {
                using(Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    dbSession.Subscriptions.Attach(_subscriptionDatabase);
                    _subscriptionDatabase.TotalBought = DateTime.Now.GetUnixTimestamp() - _subscriptionDatabase.LengthSkipped + (int)value.TotalSeconds;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion

        #region Property: Expired
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Expired
        {
            get
            {
                if (_subscriptionDatabase.LengthSkipped == 0)
                    return new TimeSpan(0); // Not started yet.

                return new TimeSpan(0, 0,
                                    DateTime.Now.GetUnixTimestamp() - _subscriptionDatabase.LengthSkipped);
            }
            set
            {
                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    dbSession.Subscriptions.Attach(_subscriptionDatabase);
                    _subscriptionDatabase.LengthSkipped = DateTime.Now.GetUnixTimestamp() - (int)value.TotalSeconds;
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion

        #region Property: IsActive
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _subscriptionDatabase.PausedStart == 0;
            }
            set
            {
                if (IsActive == value)
                    return; // Nothing to do.

                using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                {
                    dbSession.Subscriptions.Attach(_subscriptionDatabase);

                    if (!value)
                    {
                        _subscriptionDatabase.PausedStart = DateTime.Now.GetUnixTimestamp();
                    }
                    else
                    {
                        _subscriptionDatabase.LengthSkipped += DateTime.Now.GetUnixTimestamp() - _subscriptionDatabase.PausedStart;
                        _subscriptionDatabase.PausedStart = 0;
                    }
                    dbSession.SaveChanges();
                }
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Method: SubscriptionData (Constructor)
        public SubscriptionData(Bluedot.HabboServer.Habbos.Habbo habbo, string type)
        {
            Subscriber = habbo;
            Type = type;
        }
        #endregion

        #region Method: Reload
        /// <summary>
        /// Causes the subscription data to be reloaded from the database.
        /// </summary>
        public SubscriptionData Reload()
        {
            using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
            {
                _subscriptionDatabase = dbSession
                    .Subscriptions
                    .Where(subscription => subscription.SubscriberId == Subscriber.Id &&
                                           subscription.Type == Type)
                    .DefaultIfEmpty(null)
                    .SingleOrDefault();
                if (_subscriptionDatabase != null)
                    return this;

                _subscriptionDatabase = new DBSubscription
                {
                    SubscriberId = Subscriber.Id,
                    Type = Type
                };
                dbSession.Subscriptions.AddObject(_subscriptionDatabase);

                dbSession.SaveChanges();
            }
            return this;
        }
        #endregion

        #endregion
    }
}