using System.Text;
using Bluedot.HabboServer.Habbos.Figure;
namespace Bluedot.HabboServer.Habbos
{
    public abstract class FigureWaistAccessory : FigurePart
    {
        public override string ToString(bool prefixRequired)
        {
            StringBuilder sb = new StringBuilder();

            // TODO: Correct these part characters below
            sb.Append(prefixRequired ? ".HERE-" : "HERE-");

            sb.Append(ModelId);

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