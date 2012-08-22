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

using System.Text;

#endregion

namespace Bluedot.HabboServer.Figures
{
    public abstract class HumanFigure : Figure
    {
        #region Properties
        #region Property: Gender
        /// <summary>
        ///   The gender of the user.
        ///   Male = True
        ///   Female = False
        /// </summary>
        public bool Gender
        {
            get;
            set;
        }
        #endregion
        #region Property: GenderChar
        public char GenderChar
        {
            get
            {
                return Gender ? 'M' : 'F';
            }
            set
            {
                Gender = (value == 'M' ? false : true);
            }
        }
        #endregion
        #endregion
    }
}