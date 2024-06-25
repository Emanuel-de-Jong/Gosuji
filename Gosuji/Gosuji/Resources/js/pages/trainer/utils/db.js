import { settings } from "./settings";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { trainerG } from "./trainerG";
import { gameplay } from "../gameplay";

let db = { id: "db" };


db.OPENING_RATIO_MOVENUMBER = 40;
db.MIDGAME_RATIO_MOVENUMBER = 120;


db.init = function () { };

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
            "SaveTrainerSettingConfig",
                // boardsize: settings.boardsize,
                // handicap: settings.handicap,
                // colorType: settings.colorType,
                // preMovesSwitch: settings.preMovesSwitch,
                // preMoves: settings.preMoves,
                // preVisits: settings.preVisits,
                // selfplayVisits: settings.selfplayVisits,
                // suggestionVisits: settings.suggestionVisits,
                // opponentVisits: settings.opponentVisits,
                // disableAICorrection: settings.disableAICorrection,

            settings.boardsize,
            settings.handicap,
            settings.colorType,
            settings.preMovesSwitch,
            settings.preMoves,
            settings.preVisits,
            settings.selfplayVisits,
            settings.suggestionVisits,
            settings.opponentVisits,
            settings.disableAICorrection,

            settings.ruleset,
            settings.komiChangeStyle,
            settings.komi,

            settings.preOptions,
            settings.preOptionPerc,
            settings.forceOpponentCorners,
            settings.cornerSwitch44,
            settings.cornerSwitch34,
            settings.cornerSwitch33,
            settings.cornerSwitch45,
            settings.cornerSwitch35,
            settings.cornerChance44,
            settings.cornerChance34,
            settings.cornerChance33,
            settings.cornerChance45,
            settings.cornerChance35,

            settings.suggestionOptions,
            settings.showOptions,
            settings.showWeakerOptions,
            settings.minVisitsPercSwitch,
            settings.minVisitsPerc,
            settings.maxVisitDiffPercSwitch,
            settings.maxVisitDiffPerc,

            settings.opponentOptionsSwitch,
            settings.opponentOptions,
            settings.opponentOptionPerc,
            settings.showOpponentOptions
        )
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
