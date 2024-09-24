using Gosuji.Client.Data;
using Gosuji.Client.Services.TrainerService;

namespace Gosuji.Client.Services.Trainer
{
    public class LoadGameResponse
    {
        public Game Game { get; set; }
        public MoveTree MoveTree { get; set; }
        public TrainerSettingConfig TrainerSettingConfig { get; set; }

        public LoadGameResponse() { }

        public LoadGameResponse(Game game, TrainerSettingConfig trainerSettingConfig, MoveTree moveTree)
        {
            Game = game;
            TrainerSettingConfig = trainerSettingConfig;
            MoveTree = moveTree;
        }
    }
}
