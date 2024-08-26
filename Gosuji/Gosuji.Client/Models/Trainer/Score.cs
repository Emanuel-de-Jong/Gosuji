namespace Gosuji.Client.Models.Trainer
{
    public class Score
    {
        public int Winrate { get; set; }
        public int ScoreLead { get; set; }

        public string FormatWinrate(bool shouldReverse = false)
        {
            return ((shouldReverse ? GetReverseWinrate() : Winrate) / 1_000_000.0).ToString("F2");
        }

        public string FormatScoreLead(bool shouldReverse = false)
        {
            return ((shouldReverse ? GetReverseScoreLead() : ScoreLead) / 1_000_000.0).ToString("F1");
        }

        public int GetReverseWinrate()
        {
            return 100_000_000 - Winrate;
        }

        public int GetReverseScoreLead()
        {
            return ScoreLead * -1;
        }
    }
}
