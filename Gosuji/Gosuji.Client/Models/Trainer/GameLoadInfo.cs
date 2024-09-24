using Gosuji.Client.Data;
using Gosuji.Client.Services.TrainerService;

namespace Gosuji.Client.Models.Trainer
{
    public class GameLoadInfo
    {
        public string Name { get; set; }
        public EMoveColor Color { get; set; }
        public double? Result { get; set; }

        public string Ruleset { get; set; }
        public double Komi { get; set; }

        public int RightStreak { get; set; }
        public int PerfectStreak { get; set; }
        public int RightTopStreak { get; set; }
        public int PerfectTopStreak { get; set; }

        public MoveTree MoveTree { get; set; }

        public GameLoadInfo(Game game, MoveTree moveTree)
        {
            Name = game.Name;
            Color = game.Color;
            Result = game.Result;

            Ruleset = game.Ruleset;
            Komi = game.Komi;

            RightStreak = game.RightStreak;
            PerfectStreak = game.PerfectStreak;
            RightTopStreak = game.RightTopStreak;
            PerfectTopStreak = game.PerfectTopStreak;

            MoveTree = moveTree;
        }
    }
}
