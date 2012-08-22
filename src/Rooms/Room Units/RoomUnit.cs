using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.Figures;

namespace Bluedot.HabboServer.Rooms
{
    public abstract class RoomUnit : IRollerable, ITalkable
    {
        protected Queue<byte[]> _path;

        public virtual FloorPosition Destination
        {
            get;
            set;
        }
        public virtual string DisplayName
        {
            get;
            set;
        }
        public virtual Figure Figure
        {
            get;
            set;
        }
        public virtual FloorPosition Position
        {
            get;
            set;
        }
        public virtual Room Room
        {
            get;
            set;
        }

        public virtual byte[] GetNextPathStep()
        {
            if (_path.Count == 0)
                return new byte[0];

            return _path.Dequeue();
        }

        public virtual RoomUnit AddNextPathStep(byte[] step)
        {
            _path.Enqueue(step);
            return this;
        }

        public virtual RoomUnit SetPath(IEnumerable<byte[]> path)
        {
            _path = path as Queue<byte[]>;
            return this;
        }

        public virtual RoomUnit ClearPath()
        {
            _path.Clear();
            return this;
        }



        #region Implementation of IRollerable

        public abstract IRollerable Roll(FloorPosition to);
        public abstract IRollerable Roll(FloorPosition from, FloorPosition to);

        #endregion

        #region Implementation of ITalkable

        /// <summary>
        ///   Shout a message from the ITalkable to all RoomUnits in the room.
        /// </summary>
        /// <param name = "message">The message to send.</param>
        /// <returns>The current ITalkable object. This allows chaining.</returns>
        public abstract ITalkable Shout(string message);

        /// <summary>
        ///   Say a message from the ITalkable to near RoomUnits in the room.
        /// </summary>
        /// <param name = "message">The message to send.</param>
        /// <returns>The current ITalkable object. This allows chaining.</returns>
        public abstract ITalkable Say(string message);

        /// <summary>
        ///   Whisper a message from the ITalkable to another RoomUnit in the room.
        /// </summary>
        /// <param name = "recipient">The RoomUnit to recieve the message.</param>
        /// <param name = "message">The message to send.</param>
        /// <returns>The current ITalkable object. This allows chaining.</returns>
        public abstract ITalkable Whisper(ITalkable recipient, string message);

        #endregion
    }
}
