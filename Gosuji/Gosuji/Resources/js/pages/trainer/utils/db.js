import { settings } from "./settings";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { trainerG } from "./trainerG";
import { gameplay } from "../gameplay";

let db = { id: "db" };


db.OPENING_RATIO_MOVENUMBER = 40;
db.MIDGAME_RATIO_MOVENUMBER = 120;


db.init = function () {
    db.saveButton = document.getElementById("saveBtn");
    db.saveButton.addEventListener("click", db.save);
};

db.clear = function () { };


db.save = async function () {
    await db.saveTrainerSettingConfig();
    await db.saveGameStats();
    await db.saveGame();
    alert("Game saved successfully");
};

db.saveTrainerSettingConfig = async function () {
    return trainerG.trainerRef
        .invokeMethodAsync(
            "SaveTrainerSettingConfig", {
                Boardsize: settings.boardsize,
                Handicap: settings.handicap,
                ColorType: settings.colorType,
                PreMovesSwitch: settings.preMovesSwitch,
                PreMoves: settings.preMoves,
                PreVisits: settings.preVisits,
                SelfplayVisits: settings.selfplayVisits,
                SuggestionVisits: settings.suggestionVisits,
                OpponentVisits: settings.opponentVisits,
                DisableAICorrection: settings.disableAICorrection,

                Ruleset: settings.ruleset,
                KomiChangeStyle: settings.komiChangeStyle,
                Komi: settings.komi,

                PreOptions: settings.preOptions,
                PreOptionPerc: settings.preOptionPerc,
                ForceOpponentCorners: settings.forceOpponentCorners,
                CornerSwitch44: settings.cornerSwitch44,
                CornerSwitch34: settings.cornerSwitch34,
                CornerSwitch33: settings.cornerSwitch33,
                CornerSwitch45: settings.cornerSwitch45,
                CornerSwitch35: settings.cornerSwitch35,
                CornerChance44: settings.cornerChance44,
                CornerChance34: settings.cornerChance34,
                CornerChance33: settings.cornerChance33,
                CornerChance45: settings.cornerChance45,
                CornerChance35: settings.cornerChance35,

                SuggestionOptions: settings.suggestionOptions,
                ShowOptions: settings.showOptions,
                ShowWeakerOptions: settings.showWeakerOptions,
                MinVisitsPercSwitch: settings.minVisitsPercSwitch,
                MinVisitsPerc: settings.minVisitsPerc,
                MaxVisitDiffPercSwitch: settings.maxVisitDiffPercSwitch,
                MaxVisitDiffPerc: settings.maxVisitDiffPerc,

                OpponentOptionsSwitch: settings.opponentOptionsSwitch,
                OpponentOptions: settings.opponentOptions,
                OpponentOptionPerc: settings.opponentOptionPerc,
                ShowOpponentOptions: settings.showOpponentOptions
        })
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

db.saveGameStats = async function () {
    let gameRatio = stats.getRatio(0);
    let openingRatio = stats.getRatio(0, db.OPENING_RATIO_MOVENUMBER);
    let midgameRatio = stats.getRatio(db.OPENING_RATIO_MOVENUMBER + 1, db.MIDGAME_RATIO_MOVENUMBER);
    let endgameRatio = stats.getRatio(db.MIDGAME_RATIO_MOVENUMBER + 1);

    return trainerG.trainerRef
        .invokeMethodAsync("SaveGameStats", gameRatio, openingRatio, midgameRatio, endgameRatio)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

db.saveGame = async function () {
    return trainerG.trainerRef
        .invokeMethodAsync(
            "SaveGame", {
                Result: trainerG.result ? trainerG.result.scoreLead : null,
                PrevNodeX: trainerG.board.getNodeX(),
                PrevNodeY: trainerG.board.getNodeY(),
                Boardsize: trainerG.board.boardsize,
                Handicap: trainerG.board.handicap,
                Color: trainerG.color,
                Ruleset: sgf.ruleset,
                Komi: sgf.komi,
                SGF: besogo.composeSgf(trainerG.board.editor),
                Ratios: new Uint8Array(stats.encodeRatioHistory()),
                Suggestions: new Uint8Array(trainerG.suggestionsHistory.encode()),
                MoveTypes: new Uint8Array(trainerG.moveTypeHistory.encode()),
                ChosenNotPlayedCoords: new Uint8Array(gameplay.chosenNotPlayedCoordHistory.encode()),
                IsFinished: trainerG.wasPassed,
                IsThirdPartySGF: sgf.isThirdParty
            }
        )
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

export { db };
