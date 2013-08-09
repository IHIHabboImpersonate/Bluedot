using System.Text;

namespace IHI.Server.Rooms.Figure
{
    public abstract class ShirtAccessory : FigureSet
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            // TODO: Correct these part characters below
            sb.Append(prefixRequired ? ".??-" : "??-");

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