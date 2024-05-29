using Gosuji.Client.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public class GameStat : DbModel
    {
        public static Color RIGHT_COLOR = new(255, 120, 0);
        public static Color PERFECT_COLOR = new(0, 200, 0);


        [Key] public long Id { get; set; }
        public int MoveNumber { get; set; }

        public int? Winrate { get; set; }
        public int? Score { get; set; }

        public int Total { get; set; }

        public int Right { get; set; }
        public int RightStreak { get; set; }
        public int RightTopStreak { get; set; }
        [NotMapped]
        public int RightPercent => (int)((double)Right / Total * 100);

        public int Perfect { get; set; }
        public int PerfectStreak { get; set; }
        public int PerfectTopStreak { get; set; }
        [NotMapped]
        public int PerfectPercent => (int)((double)Perfect / Total * 100);

        public void Update(GameStat newModel)
        {
            MoveNumber = newModel.MoveNumber;
            Winrate = newModel.Winrate;
            Score = newModel.Score;
            Total = newModel.Total;
            Right = newModel.Right;
            RightStreak = newModel.RightStreak;
            RightTopStreak = newModel.RightTopStreak;
            Perfect = newModel.Perfect;
            PerfectStreak = newModel.PerfectStreak;
            PerfectTopStreak = newModel.PerfectTopStreak;
            base.Update(newModel);
        }

        public bool Equal(GameStat other)
        {
            return MoveNumber == other.MoveNumber &&
                Right == other.Right &&
                RightStreak == other.RightStreak &&
                RightTopStreak == other.RightTopStreak &&
                Perfect == other.Perfect &&
                PerfectStreak == other.PerfectStreak &&
                PerfectTopStreak == other.PerfectTopStreak;
        }

        public override string ToString()
        {
            return "{" +
                "\nId: " + Id +
                "\nMoveNumber: " + MoveNumber +
                "\nWinrate: " + Winrate +
                "\nScore: " + Score +
                "\nTotal: " + Total +
                "\nPerfect: " + Perfect +
                "\nPerfectTopStreak: " + PerfectTopStreak +
                "\nPerfectStreak: " + PerfectStreak +
                "\nRight: " + Right +
                "\nRightTopStreak: " + RightTopStreak +
                "\nRightStreak: " + RightStreak +
                "\n" + base.ToString() +
                "\n}";
        }
    }
}
