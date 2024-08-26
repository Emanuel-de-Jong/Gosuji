namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestion
    {
        public Coord Coord { get; set; }
        public int Visits { get; set; }
        public Score Score { get; set; }
        public string Grade { get; set; }
    }
}
