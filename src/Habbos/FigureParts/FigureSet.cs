namespace IHI.Server.Rooms.Figure
{
    public abstract class FigureSet
    {
        #region Property: PrimaryColour
        /// <summary>
        /// The primary colour of this figure part.
        /// </summary>
        public ushort PrimaryColour
        {
            get;
            set;
        }
        #endregion
        #region Property: SecondaryColour
        /// <summary>
        /// The secondary colour of this figure part.
        /// </summary>
        public ushort SecondaryColour
        {
            get;
            set;
        }
        #endregion
        
        #region Property: Id
        /// <summary>
        /// The ID of this figure set.
        /// </summary>
        public abstract ushort Id
        {
            get;
        }
        #endregion
        
        #region Property: ColourCount
        /// <summary>
        /// 
        /// </summary>
        public byte ColourCount
        {
            get
            {
                if (PrimaryColour == 0)
                    return 0;
                if (SecondaryColour == 0)
                    return 1;
                return 2;
            }
        }
        #endregion

        public abstract string ToString(bool prefixRequired);
    }
}