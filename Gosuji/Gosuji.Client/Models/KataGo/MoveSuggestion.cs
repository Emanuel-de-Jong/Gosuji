namespace Gosuji.Client.Models.KataGo
{
    public class MoveSuggestion
    {
        public Move move { get; set; }
        public int visits { get; set; }
        public int winrate { get; set; }
        public int scoreLead { get; set; }

        public MoveSuggestion() { }

        public MoveSuggestion(Move move, string visits, string winrate, string scoreLead)
        {
            SetMove(move);
            SetVisits(visits);
            SetWinrate(winrate);
            SetScoreLead(scoreLead);
        }

        public MoveSuggestion(Move move, int visits, int winrate, int scoreLead)
        {
            SetMove(move);
            this.visits = visits;
            this.winrate = winrate;
            this.scoreLead = scoreLead;
        }

        public void SetMove(Move move)
        {
            this.move = move;
        }

        public void SetVisits(string visits)
        {
            this.visits = int.Parse(visits);
        }

        public void SetWinrate(string winrate)
        {
            this.winrate = (int)(float.Parse(winrate) * 100_000_000);
            if (!move.IsBlack)
            {
                this.winrate = 100_000_000 - this.winrate;
            }
        }

        public void SetScoreLead(string scoreLead)
        {
            this.scoreLead = (int)(float.Parse(scoreLead) * 1_000_000);
            if (!move.IsBlack)
            {
                this.scoreLead *= -1;
            }
        }
    }
}
