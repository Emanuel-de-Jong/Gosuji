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
    if (g.LOG) console.log("db.saveTrainerSettingConfig");

    return trainerG.trainerRef
        .invokeMethodAsync(
            "SaveTrainerSettingConfig",
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
    if (g.LOG) console.log("db.saveGameStats");

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
    if (g.LOG) console.log("db.saveGame");

    return trainerG.trainerRef
        .invokeMethodAsync(
            "SaveGame",
            trainerG.result ? trainerG.result.scoreLead : null,
            trainerG.board.getNodeX(),
            trainerG.board.getNodeY(),
            trainerG.board.boardsize,
            trainerG.board.handicap,
            trainerG.color,
            sgf.ruleset,
            sgf.komi,
            besogo.composeSgf(trainerG.board.editor),
            new Uint8Array(stats.encodeRatioHistory()),
            new Uint8Array(trainerG.suggestionsHistory.encode()),
            new Uint8Array(trainerG.moveTypeHistory.encode()),
            new Uint8Array(gameplay.chosenNotPlayedCoordHistory.encode()),
            trainerG.wasPassed,
            sgf.isThirdParty
        )
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

export { db };
