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

namespace Bluedot.HabboServer.Habbos.Figure
{
    internal abstract class FigurePart
    {
        private ushort _primaryColour;
        private ushort _secondaryColour;

        internal ushort GetPrimaryColour()
        {
            return _primaryColour;
        }

        internal ushort GetSecondaryColour()
        {
            return _secondaryColour;
        }

        internal FigurePart SetPrimaryColour(ushort colour)
        {
            _primaryColour = colour;
            return this;
        }

        internal FigurePart SetSecondaryColour(ushort colour)
        {
            _secondaryColour = colour;
            return this;
        }


        internal abstract ushort GetModelID();

        internal byte GetAmountOfColours()
        {
            if (_primaryColour == 0)
                return 0;
            if (_secondaryColour == 0)
                return 1;
            return 2;
        }

        internal string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(prefixRequired ? ".hd-" : "hd-");


            sb.Append(GetModelID());

            if (_primaryColour != 0)
            {
                sb.Append('-');
                sb.Append(_primaryColour);

                if (_secondaryColour != 0)
                {
                    sb.Append('-');
                    sb.Append(_secondaryColour);
                }
            }

            return sb.ToString();
        }
    }
}