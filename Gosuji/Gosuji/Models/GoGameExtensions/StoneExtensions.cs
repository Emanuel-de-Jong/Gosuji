using IGOEnchi.GoGameLogic;

namespace GosujiServer.Models.GoGameWraps
{
    public static class StoneExtensions
    {
        public static string Print(this Stone stone)
        {
            string output = (stone.IsBlack ? "B" : "W");
            if (stone.X == 20)
            {
                output += "(PASS)";
            } else
            {
                output += "(" + stone.X + "," + stone.Y + ")";
            }

            return output;
        }
    }
}
