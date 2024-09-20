using Gosuji.Client.Data;
using Gosuji.Client.Models;

namespace Gosuji.Client.ViewModels
{
    public class VMGame
    {
        public string Id { get; set; }
        public GameStat? GameStat { get; set; }
        public GameStat? OpeningStat { get; set; }
        public GameStat? MidgameStat { get; set; }
        public GameStat? EndgameStat { get; set; }
        public string Name { get; set; }
        public double? Result { get; set; }
        public int RightTopStreak { get; set; }
        public int PerfectTopStreak { get; set; }
        public int Boardsize { get; set; }
        public int Handicap { get; set; }
        public EMoveColor Color { get; set; }
        public bool IsThirdPartySGF { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset ModifyDate { get; set; }
    }
}
