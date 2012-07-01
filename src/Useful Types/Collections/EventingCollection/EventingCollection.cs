using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartWeakEvent;

namespace Bluedot.HabboServer.Collections
{
    public class EventingCollection<TCollection, TItem> : ICollection<TItem>
        where TCollection: ICollection<TItem>, new()
    {
        #region Events
        #region Events: Add
        #region Event: OnPreAdd
        private readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnPreAdd = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public event EventingCollectionEventHandler<TItem> OnPreAdd
        {
            add { _eventOnPreAdd.Add(value); }
            remove { _eventOnPreAdd.Remove(value); }
        }
        #endregion
        #region Event: OnAnyPreAdd
        private static readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnAnyPreAdd = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public static event EventingCollectionEventHandler<TItem> OnAnyPreAdd
        {
            add { _eventOnAnyPreAdd.Add(value); }
            remove { _eventOnAnyPreAdd.Remove(value); }
        }
        #endregion

        #region Event: OnAdd
        private readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnAdd = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public event EventingCollectionEventHandler<TItem> OnAdd
        {
            add { _eventOnAdd.Add(value); }
            remove { _eventOnAdd.Remove(value); }
        }
        #endregion
        #region Event: OnAnyAdd
        private static readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnAnyAdd = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public static event EventingCollectionEventHandler<TItem> OnAnyAdd
        {
            add { _eventOnAnyAdd.Add(value); }
            remove { _eventOnAnyAdd.Remove(value); }
        }
        #endregion
        #endregion
        #region Events: Remove
        #region Event: OnPreRemove
        private readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnPreRemove = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public event EventingCollectionEventHandler<TItem> OnPreRemove
        {
            add { _eventOnPreRemove.Add(value); }
            remove { _eventOnPreRemove.Remove(value); }
        }
        #endregion
        #region Event: OnAnyPreRemove
        private static readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnAnyPreRemove = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public static event EventingCollectionEventHandler<TItem> OnAnyPreRemove
        {
            add { _eventOnAnyPreRemove.Add(value); }
            remove { _eventOnAnyPreRemove.Remove(value); }
        }
        #endregion

        #region Event: OnRemove
        private readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnRemove = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public event EventingCollectionEventHandler<TItem> OnRemove
        {
            add { _eventOnRemove.Add(value); }
            remove { _eventOnRemove.Remove(value); }
        }
        #endregion
        #region Event: OnAnyRemove
        private static readonly FastSmartWeakEvent<EventingCollectionEventHandler<TItem>> _eventOnAnyRemove = new FastSmartWeakEvent<EventingCollectionEventHandler<TItem>>();
        /// <summary>
        /// 
        /// </summary>
        public static event EventingCollectionEventHandler<TItem> OnAnyRemove
        {
            add { _eventOnAnyRemove.Add(value); }
            remove { _eventOnAnyRemove.Remove(value); }
        }
        #endregion
        #endregion
        #endregion

        private TCollection _internalCollection;

        #region Method: EventingCollection (Constructor)
        public EventingCollection()
        {
            _internalCollection = new TCollection();
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TItem> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(TItem item)
        {
            EventingCollectionEventArgs<TItem> eventArgs = new EventingCollectionEventArgs<TItem>(item, EventingCollectionAction.Add);
            _eventOnPreAdd.Raise(this, eventArgs);
            _eventOnAnyPreAdd.Raise(this, eventArgs);

            if (eventArgs.Cancelled)
                return;

            lock (_internalCollection)
                _internalCollection.Add(item);

            _eventOnAdd.Raise(this, eventArgs);
            _eventOnAnyAdd.Raise(this, eventArgs);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _internalCollection.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TItem item)
        {
            return _internalCollection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(TItem item)
        {
            EventingCollectionEventArgs<TItem> eventArgs = new EventingCollectionEventArgs<TItem>(item, EventingCollectionAction.Remove);
            _eventOnPreRemove.Raise(this, eventArgs);
            _eventOnAnyPreRemove.Raise(this, eventArgs);

            if (eventArgs.Cancelled)
                return false;

            bool result;
            lock (_internalCollection)
                result = _internalCollection.Remove(item);

            if (result)
            {
                _eventOnRemove.Raise(this, eventArgs);
                _eventOnAnyRemove.Raise(this, eventArgs);
            }
            return result;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _internalCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return _internalCollection.IsReadOnly; }
        }
    }
}
