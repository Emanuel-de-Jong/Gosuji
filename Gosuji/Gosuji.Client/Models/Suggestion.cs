namespace GosujiServer.Models
{
    public class Suggestion
    {
        public Coord? Coord { get; set; }
        public int Visits { get; set; }
        public Score? Score { get; set; }
    }
}
