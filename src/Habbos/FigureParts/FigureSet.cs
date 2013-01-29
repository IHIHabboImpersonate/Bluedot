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

namespace Bluedot.HabboServer.Rooms.Figure
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