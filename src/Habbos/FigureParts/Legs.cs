using System.Text;

namespace IHI.Server.Rooms.Figure
{
    public abstract class Legs : FigureSet
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            // TODO: Correct these part characters below
            sb.Append(prefixRequired ? ".lg-" : "lg-");

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