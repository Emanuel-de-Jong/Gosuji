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
};

db.saveTrainerSettingConfig = async function () {
    return trainerG.trainerRef
        .invokeMethodAsync("SaveTrainerSettingConfig")
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
                RightStreak: stats.rightStreak,
                PerfectStreak: stats.perfectStreak,
                RightTopStreak: stats.rightTopStreak,
                PerfectTopStreak: stats.perfectTopStreak,
                Boardsize: trainerG.board.boardsize,
                Handicap: trainerG.board.handicap,
                Color: trainerG.color,
                Ruleset: sgf.ruleset,
                Komi: sgf.komi,
                SGF: besogo.composeSgf(trainerG.board.editor),
                PlayerResults: new Uint8Array(stats.encodePlayerResultHistoryLoop()),
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
