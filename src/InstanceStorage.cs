
using System;
using System.Collections.Generic;
using System.Threading;

using IHI.Server.Database.Actions;

namespace IHI.Server
{
    public class InstanceStorage
    {
        private readonly Dictionary<string, byte[]> _storage = new Dictionary<string, byte[]>();
        public byte[] this[string name]
        {
            get
            {
                lock (_storage)
                {
                    byte[] data;
                    if (_storage.TryGetValue(name, out data))
                        return data;
                }
                return null;
            }
            set
            {
                lock (_storage)
                {
                    if (value == null)
                        _storage.Remove(name);
                    else
                        _storage[name] = value;
                }
            }
        }
    }
}
