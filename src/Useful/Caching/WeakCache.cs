#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General internal License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General internal License for more details.
// 
// You should have received a copy of the GNU General internal License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings
using System.Collections.Generic;
#endregion

namespace Bluedot.HabboServer.Useful
{
    public delegate TValue InstanceGenerator<in TKey, out TValue>(TKey index);

    public class WeakCache<TKey, TValue> where TValue : class
    {
        #region Fields
        #region Field: _instanceGenerator
        private readonly InstanceGenerator<TKey, TValue> _instanceGenerator;
        #endregion
        #region Field: _cache
        /// <summary>
        ///   Stores the cached instances.
        /// </summary>
        //private readonly Dictionary<TKey, WeakReference<TValue>> _cache = new Dictionary<TKey, WeakReference<TValue>>();
        private readonly WeakDictionary<TKey, TValue> _cache = new WeakDictionary<TKey, TValue>();

        public WeakCache(InstanceGenerator<TKey, TValue> instanceGenerator)
        {
            _instanceGenerator = instanceGenerator;
        }

        #endregion
        #endregion

        #region Indexers
        #region Indexer: TKey
        public TValue this [TKey index]
        {
            get
            {
                TValue instance;
                lock (this)
                {
                    // Is this Habbo already cached and has it not yet been collected and removed from memory?
                    if (!_cache.TryGetValue(index, out instance))
                    {
                        // Create a new instance using the implemented ConstructInstance method.
                        instance = _instanceGenerator.Invoke(index);

                        // And cache it.
                        _cache.Add(index, instance);
                    }
                }

                // Return the newly cached instance.
                return instance;
            }
            set
            {
                lock(this)
                {
                    if (_cache.ContainsKey(index))
                        return;
                    _cache.Add(index, value);
                }
            }
        }
        #endregion
        #endregion
    }
}