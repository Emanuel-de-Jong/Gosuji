namespace Gosuji.Client.Models
{
    public class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Coord coordToCompare)
        {
            if (coordToCompare == null)
            {
                return false;
            }

            return X == coordToCompare.X && Y == coordToCompare.Y;
        }
    }
}
