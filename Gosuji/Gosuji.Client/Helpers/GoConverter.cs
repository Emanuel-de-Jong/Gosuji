using Gosuji.Client.Models;

namespace Gosuji.Client.Helpers
{
    // Based on BesoGo
    public class GoConverter
    {
        private static Dictionary<char, int> COORD_FROM_KATAGO_X = new()
        {
            { 'A', 1 }, { 'B', 2 }, { 'C', 3 }, { 'D', 4 }, { 'E', 5 }, { 'F', 6 }, { 'G', 7 },
            { 'H', 8 }, { 'J', 9 }, { 'K', 10 }, { 'L', 11 }, { 'M', 12 }, { 'N', 13 }, { 'O', 14 },
            { 'P', 15 }, { 'Q', 16 }, { 'R', 17 }, { 'S', 18 }, { 'T', 19 }
        };
        private static char[] COORD_TO_KATAGO_X = { 'x', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' };


        public static int ColorFromKataGo(string kataGoColor)
        {
            return kataGoColor == "B" ? -1 : 1;
        }

        public static int ColorFromIGOEnchi(bool isBlack)
        {
            return isBlack ? -1 : 1;
        }

        public static Coord CoordFromKataGo(string kataGoCoord, int boardsize)
        {
            return new(COORD_FROM_KATAGO_X[kataGoCoord[0]], boardsize + 1 - int.Parse(kataGoCoord.Substring(1)));
        }

        public static Coord CoordFromIGOEnchi(int x, int y)
        {
            return new(x + 1, y + 1);
        }

        public static Move FromKataGo(string kataGoColor, string kataGoCoord, int boardsize)
        {
            return new(ColorFromKataGo(kataGoCoord), CoordFromKataGo(kataGoColor, boardsize));
        }

        public static Move FromIGOEnchi(bool isBlack, int x, int y)
        {
            return new(ColorFromIGOEnchi(isBlack), CoordFromIGOEnchi(x, y));
        }




        public static string ColorToKataGo(int color)
        {
            return color == -1 ? "B" : "W";
        }

        public static bool ColorToIGOEnchi(int color)
        {
            return color == -1 ? true : false;
        }

        public static string CoordToKataGo(Coord coord, int boardsize)
        {
            return $"{COORD_TO_KATAGO_X[coord.X]}{boardsize + 1 - coord.Y}";
        }

        public static Coord CoordToIGOEnchi(Coord coord)
        {
            return new(coord.X - 1, coord.Y - 1);
        }
    }

    public class Move
    {
        public int Color { get; set; }
        public Coord Coord { get; set; }

        public Move () { }

        public Move(int color, Coord coord)
        {
            Color = color;
            Coord = coord;
        }
    }
}
