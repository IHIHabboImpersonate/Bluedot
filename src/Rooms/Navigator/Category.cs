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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Bluedot.HabboServer.Rooms.Navigator

{
    public class Category : NavigatorListing, IEnumerator<NavigatorListing>
    {
        #region Properties

        public string IdString { get; internal set; }

        public bool IsPublicCategory
        {
            get;
            private set;
        }
        public new Category PrimaryCategory
        {
            set
            {
                base.PrimaryCategory = value;
                if (value != null)
                    IsPublicCategory = value.IsPublicCategory;
            }
            get { return base.PrimaryCategory; }
        }
        public NavigatorManager NavigatorManager
        {
            get;
            private set;
        }


        /// <summary>
        ///   The current population of the category.
        /// </summary>
        public override int Population
        {
            get
            {
                return NavigatorManager.GetChildren(this).Sum(c => c.ContainedPopulation) + ContainedPopulation;
            }
            set
            {
                throw new NotSupportedException("Setting the population of a category directly is not supported!");
            }
        }

        /// <summary>
        ///   The current population of the category.
        /// </summary>
        public override int Capacity
        {
            get
            {
                return NavigatorManager.GetChildren(this).Sum(c => c.ContainedCapacity) + ContainedCapacity;
            }
            set
            {
                throw new NotSupportedException("Setting the population of a category directly is not supported!");
            }
        }

        /// <summary>
        ///   The current population of the category.
        /// </summary>
        internal int ContainedPopulation
        {
            get;
            set;
        }

        /// <summary>
        ///   The current capacity of the category.
        /// </summary>
        internal int ContainedCapacity
        {
            get;
            set;
        }

        #endregion

        #region Fields

        private readonly List<NavigatorListing> _listings;

        #endregion

        #region Constructors

        public Category(NavigatorManager manager)
        {
            _listings = new List<NavigatorListing>();
            NavigatorManager = manager;
        }

        public Category(NavigatorManager manager, bool isPublicCategory) : this(manager)
        {
            IsPublicCategory = isPublicCategory;
        }

        #endregion

        #region Methods

        internal Category AddListing(NavigatorListing navigatorListing)
        {
            _listings.Add(navigatorListing);
            
            if (navigatorListing is Category)
                NavigatorManager.LinkCategory(navigatorListing as Category, this);
            else
            {
                ContainedPopulation += navigatorListing.Population;
                ContainedCapacity += navigatorListing.Capacity;
            }

            return this;
        }

        internal Category RemoveListing(NavigatorListing navigatorListing)
        {
            _listings.Remove(navigatorListing);
            
            if (navigatorListing is Category)
                NavigatorManager.UnlinkCategory(navigatorListing as Category);
            else
            {
                ContainedPopulation -= navigatorListing.Population;
                ContainedCapacity -= navigatorListing.Capacity;
            }
            return this;
        }

        internal bool ContainsListing(NavigatorListing navigatorListing)
        {
            return _listings.Contains(navigatorListing);
        }

        public ICollection<NavigatorListing> GetListings()
        {
            return _listings;
        }

        #endregion

        #region ListingEnumerator

        /// <summary>
        ///   Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///   true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref = "T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            return GetListings().GetEnumerator().MoveNext();
        }

        /// <summary>
        ///   Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref = "T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public void Reset()
        {
            GetListings().GetEnumerator().Reset();
        }

        /// <summary>
        ///   Gets the current element in the collection.
        /// </summary>
        /// <returns>
        ///   The current element in the collection.
        /// </returns>
        /// <exception cref = "T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
        /// <filterpriority>2</filterpriority>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        ///   Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        ///   The element in the collection at the current position of the enumerator.
        /// </returns>
        public NavigatorListing Current
        {
            get { return GetListings().GetEnumerator().Current; }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            GetListings().GetEnumerator().Dispose();
        }

        #endregion
    }
}