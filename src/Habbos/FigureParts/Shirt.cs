using System.Text;

namespace IHI.Server.Rooms.Figure
{
    public abstract class Shirt : FigureSet
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(prefixRequired ? ".ch-" : "ch-");

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