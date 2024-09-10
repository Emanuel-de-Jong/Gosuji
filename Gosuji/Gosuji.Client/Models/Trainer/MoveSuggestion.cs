namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestion
    {
        public Coord Coord { get; set; }
        public int Visits { get; set; }
        public Score Score { get; set; }
        public string? Grade { get; set; }

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

            Score.Winrate = (int)(float.Parse(winrate) * 100_000_000);
            if (color == EMoveColor.WHITE)
            {
                Score.Winrate = 100_000_000 - Score.Winrate;
            }
        }

        public void SetScoreLead(string scoreLead, EMoveColor color)
        {
            Score ??= new Score();

            Score.ScoreLead = (int)(float.Parse(scoreLead) * 1_000_000);
            if (color == EMoveColor.WHITE)
            {
                Score.ScoreLead *= -1;
            }
        }
    }
}
