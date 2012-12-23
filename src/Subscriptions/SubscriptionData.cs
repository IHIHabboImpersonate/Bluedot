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
using Bluedot.HabboServer.Database.Actions;

#endregion

namespace Bluedot.HabboServer.Habbos
{
    public class SubscriptionData
    {
        #region Fields

        #region Field: _lengthSkipped
        /// <summary>
        /// The total amount of time the subscription has previously spent paused.
        /// </summary>
        private int _lengthSkipped;
        #endregion

        #region Field: _pausedStart
        /// <summary>
        /// The timestamp that the subscription was paused on. If this is 0 then the subscription is not paused.
        /// </summary>
        private int _pausedStart;
        #endregion

        #region Field: _totalBought
        /// <summary>
        /// The total amount of time owned that of the subscription.
        /// </summary>
        private int _totalBought;
        #endregion

        #endregion

        #region Properties

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
                if (_lengthSkipped == 0)
                    return new TimeSpan(0, 0, _totalBought); // Not started yet.

                return new TimeSpan(0, 0, DateTime.Now.GetUnixTimestamp() - _totalBought + _lengthSkipped);
            }
            set
            {
                _totalBought = DateTime.Now.GetUnixTimestamp() - _lengthSkipped + (int)value.TotalSeconds;
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
                if (_lengthSkipped == 0)
                    return new TimeSpan(0); // Not started yet.

                return new TimeSpan(0, 0, DateTime.Now.GetUnixTimestamp() - _lengthSkipped);
            }
            set
            {
                _lengthSkipped = DateTime.Now.GetUnixTimestamp() - (int)value.TotalSeconds;
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
                return _pausedStart == 0;
            }
            set
            {
                if (IsActive == value)
                    return; // Nothing to do.

                if (!value)
                {
                    _pausedStart = DateTime.Now.GetUnixTimestamp();
                }
                else
                {
                    _lengthSkipped += DateTime.Now.GetUnixTimestamp() - _pausedStart;
                    _pausedStart = 0;
                }
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Method: SubscriptionData (Constructor)
        public SubscriptionData(Habbo habbo, string type)
        {
            Subscriber = habbo;
            Type = type;
        }
        #endregion
        #region Method: ~SubscriptionData (Destructor)
        ~SubscriptionData()
        {
            Save();
        }
        #endregion

        #region Method: Reload
        /// <summary>
        /// Causes the subscription data to be reloaded from the database.
        /// </summary>
        public SubscriptionData Reload()
        {
            int totalBought;
            int lengthSkipped;
            int pausedStart;

            if (SubscriptionActions.GetSubscriptionData(Subscriber.Id, Type, out totalBought, out lengthSkipped, out pausedStart))
            {
                _totalBought = totalBought;
                _lengthSkipped = lengthSkipped;
                _pausedStart = pausedStart;
            }
            return this;
        }
        #endregion
        #region Method: Save
        /// <summary>
        /// Causes the subscription data to be saved to the database.
        /// </summary>
        public SubscriptionData Save()
        {
            SubscriptionActions.SetSubscriptionData(Subscriber.Id, Type, _totalBought, _lengthSkipped, _pausedStart);
            return this;
        }
        #endregion

        #endregion
    }
}