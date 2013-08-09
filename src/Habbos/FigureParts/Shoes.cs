using System.Text;

namespace IHI.Server.Rooms.Figure
{
    public abstract class Shoes : FigureSet
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            // TODO: Correct these part characters below
            sb.Append(prefixRequired ? ".HERE-" : "HERE-");

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