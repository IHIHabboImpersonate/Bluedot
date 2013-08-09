using IHI.Server.Useful;

namespace IHI.Server.Rooms
{
    public class RoomUnitMoveEventArgs : CancelReasonEventArgs
    {
        #region Properties
        #region Property: OldPosition
        public RoomPosition OldPosition
        {
            get;
            private set;
        }
        #endregion
        #region Property: NewPosition
        public RoomPosition NewPosition
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: RoomUnitMoveEventArgs (Constructor)
        public RoomUnitMoveEventArgs(RoomPosition oldPosition, RoomPosition newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
        #endregion
        #endregion
    }
}
