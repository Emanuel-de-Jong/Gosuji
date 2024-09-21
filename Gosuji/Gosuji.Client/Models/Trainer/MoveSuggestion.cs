namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestion
    {
        public const int MAX_CONTINUATION_SIZE = 10;

        public Coord Coord { get; set; }
        public int Visits { get; set; }
        public Score Score { get; set; }
        public string? Grade { get; set; }
        public List<Coord> Continuation { get; set; } = [];
        public bool IsPass => Move.IsPass(Coord);

        public MoveSuggestion() { }

        public MoveSuggestion(Move move)
        {
            Coord = move.Coord;
        }

        public MoveSuggestion(Coord coord, int visits, Score score)
        {
            Coord = coord;
            Visits = visits;
            Score = score;
        }

        public MoveSuggestion(int x, int y, int visits, int winrate, int scoreLead)
        {
            Coord = new Coord(x, y);
            Visits = visits;
            Score = new Score(winrate, scoreLead);
        }

        public void SetVisits(string visits)
        {
            Visits = int.Parse(visits);
        }

        public void SetWinrate(string winrate, EMoveColor color)
        {
            Score ??= new Score();

            Score.Winrate = double.Parse(winrate) * 100.0;
            if (color == EMoveColor.WHITE)
            {
                Score.Winrate = Score.GetReverseWinrate();
            }
        }

        public void SetScoreLead(string scoreLead, EMoveColor color)
        {
            Score ??= new Score();

            Score.ScoreLead = double.Parse(scoreLead);
            if (color == EMoveColor.WHITE)
            {
                Score.ScoreLead = Score.GetReverseScoreLead();
            }
        }
    }
}
