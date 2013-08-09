namespace IHI.Server.Rooms.Figure
{
    public abstract class RoomUnitFigure
    {
        #region Methods
        #region Method: ToString

        public abstract override string ToString();
        #endregion
        #region Method: GetHashCode
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        #endregion
        #endregion
    }
}