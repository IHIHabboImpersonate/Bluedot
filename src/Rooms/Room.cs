using System;
using System.Collections.Generic;
using System.Drawing;

using Bluedot.HabboServer.ApiUsage.Libraries.Subscriptions;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;

namespace Bluedot.HabboServer.Rooms
{
    public abstract class Room : IMessageable, IPersistableStorage
    {
        #region Properties
        #region Property: Id
        /// <summary>
        /// The ID of this Room.
        /// This value is read only.
        /// </summary>
        public int Id { get; private set; }
        #endregion
        #region Property: Name
        /// <summary>
        /// The name of this Room.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion
        #region Property: Description
        /// <summary>
        /// The description of this Room.
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        #endregion

        #region Property: Population
        private int _population;
        /// <summary>
        /// The population of this Room.
        /// </summary>
        public int Population
        {
            get
            {
                return _population;
            }
            set
            {
                _population = value;
            }
        }
        #endregion
        #region Property: Capacity
        /// <summary>
        /// The capacity of this Room.
        /// </summary>
        public int Capacity
        {
            get;
            set;
        }
        #endregion

        #region Property: RoomUnits
        private readonly HashSet<IRoomUnit> _roomUnits;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IRoomUnit> RoomUnits
        {
            get
            {
                return _roomUnits;
            }
        }
        #endregion
        
        #region Property: PersistentStorage
        private PersistentStorage _persistentStorage;
        /// <summary>
        /// Document
        /// </summary>
        /// <remarks>Uses lazy loading.</remarks>
        public PersistentStorage PersistentStorage
        {
            get
            {
                return _persistentStorage;
            }
        }
        #endregion
        #region Property: PersistInstanceId
        public long PersistableInstanceId
        {
            get
            {
                return Id;
            }
        }
        #endregion

        #region Property: Size
        public abstract Size Size
        {
            get;
        }
        #endregion

        #region Property: BaseHeightMap
        public abstract sbyte[,] BaseHeightMap
        {
            get;
        }
        #endregion
        #region Property: Door
        public abstract RoomPosition Door
        {
            get;
        }
        #endregion

        #region Property: ModelName
        public abstract string ModelName
        {
            get;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: Room (Constructor)
        protected Room(int id)
        {
            Id = id;
            Description = String.Empty;
            Capacity = 25;

            _roomUnits = new HashSet<IRoomUnit>();
            _persistentStorage = new PersistentStorage(this);
        }
        #endregion
        
        #region Method: SendMessage
        public IMessageable SendMessage(IInternalOutgoingMessage message)
        {
            foreach (IRoomUnit roomUnit in RoomUnits)
            {
                IMessageable messageableRoomUnit = roomUnit as IMessageable;
                if (messageableRoomUnit != null)
                    messageableRoomUnit.SendMessage(message);
            }
            return this;
        }
        #endregion

        #region Method: AddRoomUnit
        public Room AddRoomUnit(IRoomUnit roomUnit)
        {
            // TODO: Code this method.
            throw new NotImplementedException();
        }
        #endregion
        #region Method: AddRoomUnit
        public Room RemoveRoomUnit(IRoomUnit roomUnit)
        {
            // TODO: Code this method.
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}