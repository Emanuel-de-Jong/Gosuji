using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Game : DbModel
    {
        [Key] public long Id { get; set; }

        [MaxLength(100)]
        public string UserId { get; set; }
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public long KataGoVersionId { get; set; }
        public KataGoVersion? kataGoVersion { get; set; }
        public long? GameStatId { get; set; }
        public GameStat? GameStat { get; set; }
        public long? OpeningStatId { get; set; }
        public GameStat? OpeningStat { get; set; }
        public long? MidgameStatId { get; set; }
        public GameStat? MidgameStat { get; set; }
        public long? EndgameStatId { get; set; }
        public GameStat? EndgameStat { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }
        public int? Result { get; set; }
        public int PrevNodeX { get; set; }
        public int PrevNodeY { get; set; }

        public int Boardsize { get; set; }
        public int Handicap { get; set; }
        public int Color { get; set; }
        [MaxLength(100)]
        public string Ruleset { get; set; }
        public float Komi { get; set; }

        [MaxLength(100_000)]
        public string SGF { get; set; }
        public byte[] Ratios { get; set; }
        public byte[] Suggestions { get; set; }
        public byte[] MoveTypes { get; set; }
        public byte[] ChosenNotPlayedCoords { get; set; }

        public bool IsFinished { get; set; }
        public bool IsThirdPartySGF { get; set; }
        public bool IsDeleted { get; set; }

        public void Update(Game newModel)
        {
            UserId = newModel.UserId;
            TrainerSettingConfigId = newModel.TrainerSettingConfigId;
            TrainerSettingConfig = newModel.TrainerSettingConfig;
            KataGoVersionId = newModel.KataGoVersionId;
            kataGoVersion = newModel.kataGoVersion;
            GameStatId = newModel.GameStatId;
            GameStat = newModel.GameStat;
            OpeningStatId = newModel.OpeningStatId;
            OpeningStat = newModel.OpeningStat;
            MidgameStatId = newModel.MidgameStatId;
            MidgameStat = newModel.MidgameStat;
            EndgameStatId = newModel.EndgameStatId;
            EndgameStat = newModel.EndgameStat;
            Name = newModel.Name;
            Result = newModel.Result;
            PrevNodeX = newModel.PrevNodeX;
            PrevNodeY = newModel.PrevNodeY;
            Boardsize = newModel.Boardsize;
            Handicap = newModel.Handicap;
            Color = newModel.Color;
            Ruleset = newModel.Ruleset;
            Komi = newModel.Komi;
            SGF = newModel.SGF;
            Ratios = newModel.Ratios;
            Suggestions = newModel.Suggestions;
            MoveTypes = newModel.MoveTypes;
            ChosenNotPlayedCoords = newModel.ChosenNotPlayedCoords;
            IsFinished = newModel.IsFinished;
            IsThirdPartySGF = newModel.IsThirdPartySGF;
            IsDeleted = newModel.IsDeleted;
            base.Update(newModel);
        }

        public override string ToString()
        {
            return "{" +
                "\nId: " + Id +
                "\nTrainerSettingConfigId: " + TrainerSettingConfigId +
                (TrainerSettingConfig == null ? "" : "\nTrainerSettingConfig: " + TrainerSettingConfig) +
                "\nOpeningStatId: " + OpeningStatId +
                (OpeningStat == null ? "" : "\nOpeningStat: " + OpeningStat) +
                "\nMidgameStatId: " + MidgameStatId +
                (MidgameStat == null ? "" : "\nMidgameStat: " + MidgameStat) +
                "\nEndgameStatId: " + EndgameStatId +
                (EndgameStat == null ? "" : "\nEndgameStat: " + EndgameStat) +
                "\nName: " + Name +
                "\nPrevNodeX: " + PrevNodeX +
                "\nPrevNodeY: " + PrevNodeY +
                "\nIsDeleted: " + IsDeleted +
                "\n" + base.ToString() +
                "\n}";
        }
    }
}
