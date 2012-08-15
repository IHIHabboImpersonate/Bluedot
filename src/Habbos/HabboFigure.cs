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
    public class HabboFigure
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

        #region Property: Body
        public Body Body
        {
            get;
            set;
        }
        #endregion
        #region Property: Hair
        public Hair Hair
        {
            get;
            set;
        }
        #endregion
        #region Property: Shirt
        public Shirt Shirt
        {
            get;
            set;
        }
        #endregion
        #region Property: Legs
        public Legs Legs
        {
            get;
            set;
        }
        #endregion
        #region Property: Shoes
        public Shoes Shoes
        {
            get;
            set;
        }
        #endregion
        #region Property: Hat
        public Hat Hat
        {
            get;
            set;
        }
        #endregion
        #region Property: EyeAccessory
        public EyeAccessory EyeAccessory
        {
            get;
            set;
        }
        #endregion
        #region Property: HeadAccessory
        public HeadAccessory HeadAccessory
        {
            get;
            set;
        }
        #endregion
        #region Property: FaceAccessory
        public FaceAccessory FaceAccessory
        {
            get;
            set;
        }
        #endregion
        #region Property: ShirtAccessory
        public ShirtAccessory ShirtAccessory
        {
            get;
            set;
        }
        #endregion
        #region Property: WaistAccessory
        public WaistAccessory WaistAccessory
        {
            get;
            set;
        }
        #endregion
        #region Property: Jacket
        public Jacket Jacket
        {
            get;
            set;
        }
        #endregion

        #region Property: SwimFigure
        private byte[] _swimFigure = new byte[]{128, 128, 128};
        /// <summary>
        ///   A byte array containing 3 values.
        ///   The values are the RGB colour values of the swim figure.
        /// </summary>
        public byte[] SwimFigure
        {
            get
            {
                return _swimFigure;
            }
            set
            {
                _swimFigure = value;
            }
        }
        #endregion
        #region Property: FormattedSwimFigure
        public string FormattedSwimFigure
        {
            get
            {
                return "ch=s0" + (Gender ? '1' : '2') + "/" + _swimFigure[0] + "," + _swimFigure[1] + "," + _swimFigure[2];
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: ToString
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool prefixRequired = false;

            if (Body != null)
            {
                stringBuilder.Append(Body.ToString(false));
                prefixRequired = true;
            }
            if (EyeAccessory != null)
            {
                stringBuilder.Append(EyeAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (FaceAccessory != null)
            {
                stringBuilder.Append(FaceAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Hair != null)
            {
                stringBuilder.Append(Hair.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Hat != null)
            {
                stringBuilder.Append(Hat.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (HeadAccessory != null)
            {
                stringBuilder.Append(HeadAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Jacket != null)
            {
                stringBuilder.Append(Jacket.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Legs != null)
            {
                stringBuilder.Append(Legs.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Shirt != null)
            {
                stringBuilder.Append(Shirt.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Shirt != null)
            {
                stringBuilder.Append(Shirt.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (Shoes != null)
            {
                stringBuilder.Append(Shoes.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (WaistAccessory != null)
            {
                stringBuilder.Append(WaistAccessory.ToString(prefixRequired));
            }

            return stringBuilder.ToString();
        }
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