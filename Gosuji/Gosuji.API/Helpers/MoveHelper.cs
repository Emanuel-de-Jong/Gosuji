using Gosuji.Client.Models;
using IGOEnchi.GoGameLogic;

namespace Gosuji.API.Helpers
{
    public class MoveHelper
    {
        public static Stone ToIGOEnchi(Move move)
        {
            Stone stone = new();

            if (move.Color != null)
            {
                stone.IsBlack = Move.ColorToIGOEnchi(move.Color.Value);
            }

            if (move.Coord != null)
            {
                Coord igoEnchiCoord = Move.CoordToIGOEnchi(move.Coord);
                stone.X = igoEnchiCoord.X;
                stone.Y = igoEnchiCoord.Y;
            }

            return stone;
        }

        public static Move FromIGOEnchi(Stone stone)
        {
            return new(Move.ColorFromIGOEnchi(stone.IsBlack), Move.CoordFromIGOEnchi(stone.X, stone.Y));
        }
    }
}
