
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



#endregion

namespace Bluedot.HabboServer.Habbos
{
    // I'm really not happy about this way of doing things.
    internal sealed class HabboIdCache : WeakCache<int, Habbo>
    {
        #region Indexers
        #region Indexer: int
        public new Habbo this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: ConstructInstance
        protected override Habbo ConstructInstance(int id)
        {
            return new Habbo(id);
        }
        #endregion
        #endregion
    }
}