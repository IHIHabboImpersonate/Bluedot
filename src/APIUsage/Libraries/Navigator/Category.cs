using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Bluedot.HabboServer.Events;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public class Category : Listing
    {
        #region Properties
        #region Property: IdString
        public string IdString { get; internal set; }
        #endregion

        #region Property: IsPublicCategory
        public bool IsPublicCategory
        {
            get;
            internal set;
        }
        #endregion

        #region Property: PrimaryCategory
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
        #endregion

        #region Property: Population
        /// <summary>
        ///   The current population of the category.
        /// </summary>
        public override int Population
        {
            get
            {
                return CoreManager.ServerCore.NavigatorManager.GetChildren(this).Sum(c => c.ContainedPopulation) + ContainedPopulation;
            }
            set
            {
                throw new NotSupportedException("Setting the population of a category directly is not supported!");
            }
        }
        #endregion
        #region Property: Capacity
        /// <summary>
        ///   The current population of the category.
        /// </summary>
        public override int Capacity
        {
            get
            {
                return CoreManager.ServerCore.NavigatorManager.GetChildren(this).Sum(c => c.ContainedCapacity) + ContainedCapacity;
            }
            set
            {
                throw new NotSupportedException("Setting the population of a category directly is not supported!");
            }
        }
        #endregion

        #region Property: ContainedPopulation
        /// <summary>
        ///   The current population of the category.
        /// </summary>
        internal int ContainedPopulation
        {
            get;
            set;
        }
        #endregion
        #region Property: ContainedCapacity
        /// <summary>
        ///   The current capacity of the category.
        /// </summary>
        internal int ContainedCapacity
        {
            get;
            set;
        }
        #endregion
        #endregion

        #region Fields
        #region Field: _listings
        private readonly List<Listing> _listings;
        #endregion
        #region Field: _
        /// <summary>
        /// 
        /// </summary>
        private ReaderWriterLockSlim _locking;
        #endregion
        #endregion

        #region Methods
        #region Method: Category (Constructor)
        public Category()
        {
            _listings = new List<Listing>();
            _locking = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }
        #endregion

        #region Method: AddListing
        internal bool AddListing(Listing listing)
        {
            if (listing == null)
                throw new ArgumentNullException("listing");

            EventManager events = CoreManager.ServerCore.EventManager;

            ListingEventArgs eventArgs = new ListingEventArgs(listing);
            events.Fire("navigator-category_add-listing:before", this, eventArgs);

            if (eventArgs.Cancel)
                return false;

            _locking.ExitWriteLock();
            try
            {
                _listings.Add(listing);
            }
            finally
            {
                _locking.ExitWriteLock();
            }

            if (listing is Category)
                CoreManager.ServerCore.NavigatorManager.AddCategory(listing as Category, this);
            else
            {
                ContainedPopulation += listing.Population;
                ContainedCapacity += listing.Capacity;
            }

            events.Fire("navigator-category_add-listing:after", this, eventArgs);
            return true;
        }
        #endregion

        #region Method: RemoveListing
        internal bool RemoveListing(Listing listing)
        {
            if (listing == null)
                throw new ArgumentNullException("listing");

            EventManager events = CoreManager.ServerCore.EventManager;
            ListingEventArgs eventArgs = new ListingEventArgs(listing);

            _locking.EnterUpgradeableReadLock();
            try
            {
                if (_listings.Contains(listing))
                    return false;

                events.Fire("navigator-category_add-listing:before", this, eventArgs);
                if (eventArgs.Cancel)
                    return false;

                _locking.EnterWriteLock();
                try
                {
                    _listings.Remove(listing);
                }
                finally
                {
                    _locking.ExitWriteLock();
                }
            }
            finally
            {
                _locking.ExitUpgradeableReadLock();
            }

            if (listing is Category)
                CoreManager.ServerCore.NavigatorManager.AddCategory(listing as Category, this);
            else
            {
                ContainedPopulation += listing.Population;
                ContainedCapacity += listing.Capacity;
            }

            events.Fire("navigator-category_add-listing:after", this, eventArgs);
            return true;
        }
        #endregion

        #region Method: ContainsListing
        internal bool ContainsListing(Listing listing)
        {
            return _listings.Contains(listing);
        }
        #endregion

        #region Method: GetListings
        public ICollection<Listing> GetListings()
        {
            return _listings;
        }
        #endregion
        #endregion
    }
}
