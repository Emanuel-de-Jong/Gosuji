import { kataGo } from "./kataGo";
import { settings } from "./settings";
import { stats } from "./stats";
import { trainerG } from "./trainerG";

let sgf = { id: "sgf" };


sgf.init = function (userName) {
    sgf.userName = userName;

    sgf.sgfLoadingEvent = new CEvent(sgf.sgfLoadingListener);
    sgf.sgfLoadedEvent = new CEvent(sgf.sgfLoadedListener);

    trainerG.board.editor.addListener(sgf.boardEditorListener);

    sgf.clear();
};

sgf.clear = function () {
    sgf.isSGFLoading = false;
    sgf.isThirdParty = false;

    sgf.setRuleset(trainerG.gameLoadInfo ? trainerG.gameLoadInfo.ruleset : settings.ruleset);
    sgf.setKomi(trainerG.gameLoadInfo ? trainerG.gameLoadInfo.komi : settings.komi);

    trainerG.board.editor.setGameInfo("Gosuji", "GN");
    trainerG.board.editor.setGameInfo("Gosuji", "SO");
    trainerG.board.editor.setGameInfo(Date(), "DT");

    sgf.setPlayersMeta();
    sgf.setHandicapMeta();
};


sgf.boardEditorListener = async function (event) {
    if (event.sgfEvent) {
        if (!event.sgfLoaded) {
            await sgf.sgfLoadingEvent.dispatchAsync();
        } else {
            await sgf.sgfLoadedEvent.dispatchAsync();
        }
    }
};

sgf.sgfLoadingListener = function () {
    sgf.isSGFLoading = true;
};

sgf.sgfLoadedListener = function () {
    sgf.isThirdParty = true;

    let gameInfo = trainerG.board.editor.getGameInfo();

    if (gameInfo.RE) {
        stats.setResult(gameInfo.RE);
    }

    trainerG.setColor();

    if (gameInfo.SZ) {
        trainerG.board.boardsize = parseInt(gameInfo.SZ);
    }

    if (gameInfo.HA) {
        trainerG.board.setHandicap(parseInt(gameInfo.HA));
    }

    if (confirm("Would you like to use the ruleset and komi of the SGF?")) {
        if (gameInfo.RU) {
            let ruleset = gameInfo.RU.toLowerCase();
            if (ruleset.includes("japan")) {
                sgf.setRuleset("Japanese");
            } else if (ruleset.includes("chin") || ruleset.includes("korea")) {
                sgf.setRuleset("Chinese");
            }
        }

        sgf.setKomi(parseFloat(gameInfo.KM));
    }

    sgf.isSGFLoading = false;
};


sgf.setRuleset = function (ruleset) {
    sgf.ruleset = ruleset;
    document.getElementById("rulesetDisplay").textContent = ruleset;
};

sgf.setKomi = function (komi) {
    sgf.komi = komi;
    trainerG.board.komiDisplay.textContent = komi;
};


sgf.setPlayersMeta = function () {
    trainerG.board.editor.setGameInfo(sgf.userName ? sgf.userName : "Player", "P" + g.colorNumToName(trainerG.color));
    trainerG.board.editor.setGameInfo("AI", "P" + g.colorNumToName(trainerG.color * -1));
};

sgf.setHandicapMeta = function () {
    trainerG.board.editor.setGameInfo(trainerG.board.handicap + "", "HA");
};

sgf.setRulesetMeta = function () {
    trainerG.board.editor.setGameInfo(sgf.ruleset, "RU");
};

sgf.setKomiMeta = function () {
    trainerG.board.editor.setGameInfo(sgf.komi + "", "KM");
};

sgf.setResultMeta = function (resultStr) {
    trainerG.board.editor.setGameInfo(resultStr, "RE");
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.sgf) window.trainer.sgf = sgf;

export { sgf };
