using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models
{
    public class Move
    {
        private static Dictionary<char, int> COORD_FROM_KATAGO_X = new()
        {
            { 'A', 1 }, { 'B', 2 }, { 'C', 3 }, { 'D', 4 }, { 'E', 5 }, { 'F', 6 }, { 'G', 7 },
            { 'H', 8 }, { 'J', 9 }, { 'K', 10 }, { 'L', 11 }, { 'M', 12 }, { 'N', 13 }, { 'O', 14 },
            { 'P', 15 }, { 'Q', 16 }, { 'R', 17 }, { 'S', 18 }, { 'T', 19 }
        };
        private static char[] COORD_TO_KATAGO_X = { 'x', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' };


        public int Color { get; set; }
        public Coord Coord { get; set; }

        public bool IsBlack => Color == -1;
        public bool IsPass => Coord.X == 0 && Coord.Y == 0;


        public Move() { }

        public Move(int color, Coord coord)
        {
            Color = color;
            Coord = coord;
        }


        public string ColorToKataGo()
        {
            return ColorToKataGo(Color);
        }

        public bool ColorToIGOEnchi()
        {
            return ColorToIGOEnchi(Color);
        }

        public string CoordToKataGo(int boardsize)
        {
            return CoordToKataGo(Coord, boardsize);
        }

        public Coord CoordToIGOEnchi()
        {
            return CoordToIGOEnchi(Coord);
        }

        public KataGoMove ToKataGo(int boardsize)
        {
            return ToKataGo(this, boardsize);
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
            if (coord.X == 0 && coord.Y == 0)
            {
                return "pass";
            }

            return $"{COORD_TO_KATAGO_X[coord.X]}{boardsize + 1 - coord.Y}";
        }

        public static Coord CoordToIGOEnchi(Coord coord)
        {
            if (coord.X == 0 && coord.Y == 0)
            {
                return new(20, 20);
            }

            return new(coord.X - 1, coord.Y - 1);
        }

        public static KataGoMove ToKataGo(Move move, int boardsize)
        {
            return new(ColorToKataGo(move.Color), CoordToKataGo(move.Coord, boardsize));
        }


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
            if (kataGoCoord == "pass")
            {
                return new(0, 0);
            }

            return new(COORD_FROM_KATAGO_X[kataGoCoord[0]], boardsize + 1 - int.Parse(kataGoCoord.Substring(1)));
        }

        public static Coord CoordFromIGOEnchi(int x, int y)
        {
            if (x == 20 && y == 20)
            {
                return new(0, 0);
            }

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
    }

    public class KataGoMove
    {
        public string Color { get; set; }
        public string Coord { get; set; }

        public KataGoMove(string color, string coord)
        {
            Color = color;
            Coord = coord;
        }
    }
}
