using System.Text;

namespace IHI.Server.Rooms.Figure
{
    public abstract class EyeAccessory : FigureSet
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(prefixRequired ? ".ey-" : "ey-");

            sb.Append(Id);

            if (PrimaryColour != 0)
            {
                sb.Append('-');
                sb.Append(PrimaryColour);

                if (SecondaryColour != 0)
                {
                    sb.Append('-');
                    sb.Append(SecondaryColour);
                }
            }

            return sb.ToString();
        }
    }
}