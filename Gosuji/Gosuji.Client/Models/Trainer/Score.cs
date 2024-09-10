namespace Gosuji.Client.Models.Trainer
{
    public class Score
    {
        public double Winrate { get; set; }
        public double ScoreLead { get; set; }

        public Score() { }

        public Score(double winrate, double scoreLead)
        {
            Winrate = winrate;
            ScoreLead = scoreLead;
        }

        public string FormatWinrate(bool shouldReverse = false)
        {
            return (shouldReverse ? GetReverseWinrate() : Winrate).ToString("F2");
        }

        public string FormatScoreLead(bool shouldReverse = false)
        {
            return (shouldReverse ? GetReverseScoreLead() : ScoreLead).ToString("F1");
        }

        public double GetReverseWinrate()
        {
            return 1.0 - Winrate;
        }

        public double GetReverseScoreLead()
        {
            return ScoreLead * -1.0;
        }
    }
}
