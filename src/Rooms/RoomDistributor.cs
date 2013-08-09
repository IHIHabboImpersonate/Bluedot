#region Usings

using System;
using System.Collections.Generic;
using System.Threading;

using IHI.Server.Database.Actions;
using IHI.Server.Habbos;
using IHI.Server.Network;
using IHI.Server.Useful;
using System.Reflection;

#endregion

namespace IHI.Server.Rooms
{
    public class RoomDistributor
    {
        #region Fields
        #region Field: _idCache
        private readonly WeakCache<int, Room> _idCache;
        #endregion
        #region Field: _overrideLoaders
        private readonly Dictionary<int, Func<Habbo, Room>> _overrideLoaders;
        #endregion

        #region Field: _lastFreeRoomId
        private int _lastFreeRoomId;
        #endregion
        #region Field: _roomIdSync
        private ReaderWriterLockSlim _roomIdSync;
        #endregion

        #region Field: _modelTypeLookup
        private Dictionary<string, Type> _modelTypeLookup;
        #endregion
        #endregion

        #region Indexers
        #region Indexer: int
        public Room this[int id]
        {
            get
            {
                return GetRoom(id);
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: RoomDistributor (Constructor)
        public RoomDistributor()
        {
            _idCache = new WeakCache<int, Room>(ConstructRoom);
            _overrideLoaders = new Dictionary<int, Func<Habbo, Room>>();

            _lastFreeRoomId = -1;
            _roomIdSync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            _modelTypeLookup = new Dictionary<string, Type>();
        }
        #endregion

        #region Method: ConstructRoom
        public Room ConstructRoom(int id)
        {
            string model = RoomActions.GetRoomModelFromRoomId(id);

            if (!_modelTypeLookup.ContainsKey(model))
                FindNewModelTypes();
            if (!_modelTypeLookup.ContainsKey(model))
                throw new Exception(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_ROOM_MODEL_MISSING", model));

            Type modelType = _modelTypeLookup[model];

            return (Room)Activator.CreateInstance(modelType, new object[] {id}); // This might be slow. Keep that in mind if performance becomes a problem.
        }
        #endregion

        #region Method: GetFreeRoomId
        public Room GetRoom(int id, Habbo habbo = null)
        {
            Func<Habbo, Room> overrideLoader = GetOverrideLoader(id);
            if (overrideLoader == null)
                return _idCache[id];

            _roomIdSync.EnterWriteLock();
            try
            {
                Room room = overrideLoader(habbo);
                _idCache.Add(room.Id, room);
                return room;
            }
            finally
            {
                _roomIdSync.ExitWriteLock();
            }
        }

        public int GetFreeRoomId()
        {
            bool reachedEnd = false;

            _roomIdSync.EnterWriteLock();
            try
            {
                while (_idCache.ContainsKey(_lastFreeRoomId) || _overrideLoaders.ContainsKey(_lastFreeRoomId))
                {
                    if (_lastFreeRoomId == int.MinValue)
                    {
                        if (!reachedEnd)
                        {
                            _lastFreeRoomId = -1;
                            reachedEnd = true;
                        }
                        else
                        {
                            throw new Exception("No spare negative RoomIDs!");
                        }
                    }
                    else
                        _lastFreeRoomId--;
                }
                return _lastFreeRoomId;
            }
            finally
            {
                _roomIdSync.ExitWriteLock();
            }
        }
        #endregion

        #region Method: AddOverrideLoader
        public int AddOverrideLoader(Func<Habbo, Room> overrideLoader)
        {
            _roomIdSync.EnterWriteLock();
            try
            {
                int roomId = GetFreeRoomId();
                _overrideLoaders.Add(roomId, overrideLoader);
                return roomId;
            }
            finally
            {
                _roomIdSync.ExitWriteLock();
            }
        }
        #endregion
        #region Method: SetOverrideLoader
        public RoomDistributor SetOverrideLoader(int roomId, Func<Habbo, Room> overrideLoader)
        {
            _roomIdSync.EnterWriteLock();
            try
            {
                if (overrideLoader == null)
                {
                    if (_overrideLoaders.ContainsKey(roomId))
                        _overrideLoaders.Remove(roomId);
                }
                else
                {
                    if (_overrideLoaders.ContainsKey(roomId))
                        _overrideLoaders[roomId] = overrideLoader;
                    else
                        _overrideLoaders.Add(roomId, overrideLoader);
                }
            }
            finally
            {
                _roomIdSync.ExitWriteLock();
            }
            return this;
        }
        #endregion
        #region Method: GetOverrideLoader
        public Func<Habbo, Room> GetOverrideLoader(int roomId)
        {
            _roomIdSync.EnterReadLock();
            try
            {
                Func<Habbo, Room> overrideLoader;
                if (_overrideLoaders.TryGetValue(roomId, out overrideLoader))
                    return overrideLoader;
                return null;
            }
            finally
            {
                _roomIdSync.ExitReadLock();
            }
        }
        #endregion

        #region Method: FindNewModelTypes
        private void FindNewModelTypes()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.IsDefined(typeof(RoomModelAttribute), true))
                    {
                        RoomModelAttribute attr = t.GetCustomAttribute<RoomModelAttribute>(true);
                        _modelTypeLookup.Add(attr.ModelName, t);
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}