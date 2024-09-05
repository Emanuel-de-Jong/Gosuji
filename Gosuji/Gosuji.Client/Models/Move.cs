using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Gosuji.Client.Models
{
    public class Move
    {
        public static Coord PASS_COORD = new(0, 0);

        public int? Color { get; set; }
        public Coord? Coord { get; set; }

        public bool IsBlack => Color == -1;
        public bool IsPass => Coord.X == 0 && Coord.Y == 0;


        public Move() { }

        public Move(Coord coord)
        {
            Coord = coord;
        }

        public Move(int x, int y)
        {
            Coord = new Coord(x, y);
        }

        public Move(int color, Coord coord)
            : this(coord)
        {
            Color = color;
        }

        public Move(int color, int x, int y)
            :this(x, y)
        {
            Color = color;
        }
    }
}
