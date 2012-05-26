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
    internal class HabboFigure // : IFigure
    {
        private Body _body;
        private EyeAccessory _eyeAccessory;
        private FaceAccessory _faceAccessory;

        /// <summary>
        ///   The gender of the user.
        ///   Male = True
        ///   Female = False
        /// </summary>
        private bool _gender;

        private Hair _hair;

        private Hat _hat;
        private HeadAccessory _headAccessory;
        private Jacket _jacket;
        private Legs _legs;
        private Shirt _shirt;
        private ShirtAccessory _shirtAccessory;
        private Shoes _shoes;
        private uint _swimFigure;
        private WaistAccessory _waistAccessory;

        internal HabboFigure(bool gender)
        {
            _gender = gender;
        }

        #region Gender

        internal bool GetGender()
        {
            return _gender;
        }

        internal char GetGenderChar()
        {
            return (_gender ? 'M' : 'F');
        }

        internal HabboFigure SetGender(bool gender)
        {
            _gender = gender;
            return this;
        }

        #endregion

        #region IFigure Members

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool prefixRequired = false;

            if (_body != null)
            {
                stringBuilder.Append(_body.ToString(false));
                prefixRequired = true;
            }
            if (_eyeAccessory != null)
            {
                stringBuilder.Append(_eyeAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_faceAccessory != null)
            {
                stringBuilder.Append(_faceAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_hair != null)
            {
                stringBuilder.Append(_hair.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_hat != null)
            {
                stringBuilder.Append(_hat.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_headAccessory != null)
            {
                stringBuilder.Append(_headAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_jacket != null)
            {
                stringBuilder.Append(_jacket.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_legs != null)
            {
                stringBuilder.Append(_legs.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_shirt != null)
            {
                stringBuilder.Append(_shirt.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_shirtAccessory != null)
            {
                stringBuilder.Append(_shirtAccessory.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_shoes != null)
            {
                stringBuilder.Append(_shoes.ToString(prefixRequired));
                prefixRequired = true;
            }
            if (_waistAccessory != null)
            {
                stringBuilder.Append(_waistAccessory.ToString(prefixRequired));
            }

            return stringBuilder.ToString();
        }

        #endregion

        internal Body GetBody()
        {
            return _body;
        }

        internal HabboFigure SetBody(Body value)
        {
            _body = value;
            return this;
        }

        internal Hair GetHair()
        {
            return _hair;
        }

        internal HabboFigure SetHair(Hair value)
        {
            _hair = value;
            return this;
        }

        internal Shirt GetShirt()
        {
            return _shirt;
        }

        internal HabboFigure SetShirt(Shirt value)
        {
            _shirt = value;
            return this;
        }

        internal Legs GeLegs()
        {
            return _legs;
        }

        internal HabboFigure SetLegs(Legs value)
        {
            _legs = value;
            return this;
        }

        internal Shoes GetShoes()
        {
            return _shoes;
        }

        internal HabboFigure SetShoes(Shoes value)
        {
            _shoes = value;
            return this;
        }


        internal Hat GetHat()
        {
            return _hat;
        }

        internal HabboFigure SetHat(Hat value)
        {
            _hat = value;
            return this;
        }

        internal EyeAccessory GetEyeAccessory()
        {
            return _eyeAccessory;
        }

        internal HabboFigure SetEyeAccessory(EyeAccessory value)
        {
            _eyeAccessory = value;
            return this;
        }

        internal HeadAccessory GetHeadAccessory()
        {
            return _headAccessory;
        }

        internal HabboFigure SetHeadAccessory(HeadAccessory value)
        {
            _headAccessory = value;
            return this;
        }

        internal FaceAccessory GetFaceAccesory()
        {
            return _faceAccessory;
        }

        internal HabboFigure SetFaceAccessory(FaceAccessory value)
        {
            _faceAccessory = value;
            return this;
        }

        internal ShirtAccessory GetShirtAccessory()
        {
            return _shirtAccessory;
        }

        internal HabboFigure SetShirtAccessories(ShirtAccessory value)
        {
            _shirtAccessory = value;
            return this;
        }

        internal WaistAccessory GetWaistAccessory()
        {
            return _waistAccessory;
        }

        internal HabboFigure SetWaistAccessory(WaistAccessory value)
        {
            _waistAccessory = value;
            return this;
        }

        internal Jacket GetJacket()
        {
            return _jacket;
        }

        internal HabboFigure SetJacket(Jacket value)
        {
            _jacket = value;
            return this;
        }


        /// <summary>
        ///   Returns a byte array containing 3 values.
        ///   The values are the RGB colour values of the swim figure.
        /// </summary>
        internal byte[] GetSwimFigure()
        {
            return new[]
                       {(byte) (_swimFigure >> 16), (byte) (_swimFigure << 8 >> 16), (byte) ((_swimFigure << 16) >> 16)};
        }

        /// <summary>
        ///   Returns a byte array containing 3 values.
        ///   The values are the RGB colour values of the swim figure.
        /// </summary>
        internal string GetFormattedSwimFigure()
        {
            return "ch=s0" + (GetGender() ? '1' : '2') + "/" + (_swimFigure >> 16) + "," + (_swimFigure << 8 >> 16) +
                   "," + ((_swimFigure << 16) >> 16);
        }

        /// <summary>
        ///   Sets the colour of the swim figure.
        /// </summary>
        /// <param name = "red">The amount of red in the colour.</param>
        /// <param name = "green">The amount of green in the colour.</param>
        /// <param name = "blue">The amount of blue in the colour.</param>
        /// <returns></returns>
        internal HabboFigure SetSwimFigure(byte red, byte green, byte blue)
        {
            _swimFigure = (uint) ((red << 16) | green << 8) | blue;
            return this;
        }
    }
}