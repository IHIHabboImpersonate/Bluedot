#region Usings

using System.Text;

#endregion

namespace IHI.Server.Rooms.Figure
{
    public class HabboFigure : RoomUnitFigure
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
                Gender = (value != 'M');
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
        #endregion
    }
}