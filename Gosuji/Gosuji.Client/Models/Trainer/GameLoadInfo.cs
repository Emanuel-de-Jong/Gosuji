using Gosuji.Client.Data;

namespace Gosuji.Client.Models.Trainer
{
    public class GameLoadInfo
    {
        public string Name { get; set; }
        public EMoveColor Color { get; set; }
        public double? Result { get; set; }
        public int PrevNodeX { get; set; }
        public int PrevNodeY { get; set; }

        public int RightStreak { get; set; }
        public int PerfectStreak { get; set; }
        public int RightTopStreak { get; set; }
        public int PerfectTopStreak { get; set; }

        public GameLoadInfo() { }

        public GameLoadInfo(Game game)
        {
            Name = game.Name;
            Color = game.Color;
            Result = game.Result;
            PrevNodeX = game.LastNodeX;
            PrevNodeY = game.LastNodeY;

            RightStreak = game.RightStreak;
            PerfectStreak = game.PerfectStreak;
            RightTopStreak = game.RightTopStreak;
            PerfectTopStreak = game.PerfectTopStreak;
        }
    }
}
