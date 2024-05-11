using IGOEnchi.GoGameLogic;

namespace GosujiServer.Models.GoGameWraps
{
    public static class StoneExtensions
    {
        public static string Print(this Stone stone)
        {
            return (stone.IsBlack ? "B" : "W") +
                "(" + stone.X + "," + stone.Y + ")";
        }
    }
}
