using Gosuji.Client.Data;
using Gosuji.Client.Helpers.GameDecoder;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.Trainer
{
    public class GameLoadInfo
    {
        public string Name { get; set; }
        public int? Result { get; set; }
        public int PrevNodeX { get; set; }
        public int PrevNodeY { get; set; }

        public int RightStreak { get; set; }
        public int PerfectStreak { get; set; }
        public int RightTopStreak { get; set; }
        public int PerfectTopStreak { get; set; }

        public int Boardsize { get; set; }
        public int Handicap { get; set; }
        public int Color { get; set; }
        public double Komi { get; set; }
        public string Ruleset { get; set; }

        public string SGF { get; set; }
        public Dictionary<short, Dictionary<short, EPlayerResult>> PlayerResults { get; set; }
        public Dictionary<short, Dictionary<short, SuggestionList>> Suggestions { get; set; }
        public Dictionary<short, Dictionary<short, EMoveType>> MoveTypes { get; set; }
        public Dictionary<short, Dictionary<short, Coord>> ChosenNotPlayedCoords { get; set; }

        public GameLoadInfo() { }

        public GameLoadInfo(Game game)
        {
            Name = game.Name;
            Result = game.Result;
            PrevNodeX = game.PrevNodeX;
            PrevNodeY = game.PrevNodeY;

            RightStreak = game.RightStreak;
            PerfectStreak = game.PerfectStreak;
            RightTopStreak = game.RightTopStreak;
            PerfectTopStreak = game.PerfectTopStreak;

            Boardsize = game.Boardsize;
            Handicap = game.Handicap;
            Color = game.Color;
            Komi = game.Komi;
            Ruleset = game.Ruleset;

            SGF = game.SGF;
            PlayerResults = GameDecoder.DecodePlayerResults(game.PlayerResults).ToDict();
            Suggestions = GameDecoder.DecodeSuggestions(game.Suggestions);
            MoveTypes = GameDecoder.DecodeMoveTypes(game.MoveTypes);
            ChosenNotPlayedCoords = GameDecoder.DecodeChosenNotPlayedCoords(game.ChosenNotPlayedCoords);
        }
    }
}
