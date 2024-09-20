namespace Gosuji.Client.Models
{
    public class Move : IEquatable<Move>
    {
        private static Dictionary<char, int> COORD_FROM_KATAGO_X = new()
        {
            { 'A', 1 }, { 'B', 2 }, { 'C', 3 }, { 'D', 4 }, { 'E', 5 }, { 'F', 6 }, { 'G', 7 },
            { 'H', 8 }, { 'J', 9 }, { 'K', 10 }, { 'L', 11 }, { 'M', 12 }, { 'N', 13 }, { 'O', 14 },
            { 'P', 15 }, { 'Q', 16 }, { 'R', 17 }, { 'S', 18 }, { 'T', 19 }
        };
        private static char[] COORD_TO_KATAGO_X = { 'x', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' };

        public static Coord PASS_COORD = new(0, 0);
        public static Move PASS = new(PASS_COORD);

        public EMoveColor? Color { get; set; }
        public Coord? Coord { get; set; }

        public int? X => Coord?.X;
        public int? Y => Coord?.Y;
        public bool IsBlack => Color == EMoveColor.BLACK;


        public Move() { }

        public Move(Coord coord)
        {
            Coord = coord;
        }

        public Move(int x, int y)
            : this(new(x, y)) { }

        public Move(EMoveColor color, Coord coord)
            : this(coord)
        {
            Color = color;
        }

        public Move(EMoveColor color, int x, int y)
            : this(x, y)
        {
            Color = color;
        }


        public string ColorToKataGo()
        {
            return ColorToKataGo(Color.Value);
        }

        public bool ColorToIGOEnchi()
        {
            return ColorToIGOEnchi(Color.Value);
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

        public bool IsPass()
        {
            return IsPass(Coord);
        }


        public static string ColorToKataGo(EMoveColor color)
        {
            return color == EMoveColor.BLACK ? "B" : "W";
        }

        public static bool ColorToIGOEnchi(EMoveColor color)
        {
            return color == EMoveColor.BLACK;
        }

        public static string CoordToKataGo(Coord coord, int boardsize)
        {
            if (coord.Equals(PASS_COORD))
            {
                return "pass";
            }

            return $"{COORD_TO_KATAGO_X[coord.X]}{boardsize + 1 - coord.Y}";
        }

        public static Coord CoordToIGOEnchi(Coord coord)
        {
            if (coord.Equals(PASS_COORD))
            {
                return new(20, 20);
            }

            return new(coord.X - 1, coord.Y - 1);
        }

        public static KataGoMove ToKataGo(Move move, int boardsize)
        {
            KataGoMove kataGoMove = new();

            if (move.Color != null)
            {
                kataGoMove.Color = ColorToKataGo(move.Color.Value);
            }

            if (move.Coord != null)
            {
                kataGoMove.Coord = CoordToKataGo(move.Coord, boardsize);
            }

            return kataGoMove;
        }


        public static EMoveColor ColorFromKataGo(string kataGoColor)
        {
            return kataGoColor == "B" ? EMoveColor.BLACK : EMoveColor.WHITE;
        }

        public static EMoveColor ColorFromIGOEnchi(bool isBlack)
        {
            return isBlack ? EMoveColor.BLACK : EMoveColor.WHITE;
        }

        public static Coord CoordFromKataGo(string kataGoCoord, int boardsize)
        {
            if (kataGoCoord == "pass")
            {
                return PASS_COORD;
            }

            return new(COORD_FROM_KATAGO_X[kataGoCoord[0]], boardsize + 1 - int.Parse(kataGoCoord[1..]));
        }

        public static Coord CoordFromIGOEnchi(int x, int y)
        {
            if (x == 20 && y == 20)
            {
                return PASS_COORD;
            }

            return new(x + 1, y + 1);
        }

        public static Move FromKataGo(string kataGoColor, string kataGoCoord, int boardsize)
        {
            return new(ColorFromKataGo(kataGoCoord), CoordFromKataGo(kataGoColor, boardsize));
        }

        public static bool IsPass(Coord? coord)
        {
            if (coord == null)
            {
                return false;
            }

            return coord.Equals(PASS_COORD);
        }

        public bool Equals(Move other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Color == other.Color &&
                Coord != null && Coord.Equals(other.Coord);
        }
    }

    public class KataGoMove
    {
        public string? Color { get; set; }
        public string? Coord { get; set; }

        public KataGoMove() { }
    }

    public enum EMoveColor
    {
        BLACK = -1,
        RANDOM = 0,
        WHITE = 1,
    }
}
