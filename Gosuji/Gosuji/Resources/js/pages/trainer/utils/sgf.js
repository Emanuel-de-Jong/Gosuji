import { katago } from "./katago";
import { settings } from "./settings";
import { stats } from "./stats";
import { trainerG } from "./trainerG";

let sgf = { id: "sgf" };


sgf.init = async function (userName, serverKomi, serverRuleset) {
    sgf.rulesetElement = document.getElementById("currentRuleset");
    sgf.komiElement = document.getElementById("currentKomi");

    sgf.userName = userName;

    sgf.sgfLoadingEvent = new CEvent(sgf.sgfLoadingListener);
    sgf.sgfLoadedEvent = new CEvent(sgf.sgfLoadedListener);

    await sgf.clear(serverKomi, serverRuleset);
};

sgf.clear = async function (serverKomi, serverRuleset) {
    sgf.isSGFLoading = false;
    sgf.isThirdParty = false;

    await sgf.setRuleset(serverKomi != null ? serverKomi : settings.ruleset);
    await sgf.setKomi(serverRuleset ? serverRuleset : settings.komi);

    trainerG.board.editor.setGameInfo("GoTrainer-HumanAI", "GN");
    trainerG.board.editor.setGameInfo("GoTrainer-HumanAI", "SO");
    trainerG.board.editor.setGameInfo(Date(), "DT");

    sgf.setPlayersMeta();
    sgf.setRankPlayerMeta();
    sgf.setRankAIMeta();
    sgf.setHandicapMeta();
};


sgf.boardEditorListener = function (event) {
    if (event.sgfEvent) {
        if (!event.sgfLoaded) {
            sgf.sgfLoadingEvent.dispatch();
        } else {
            sgf.sgfLoadedEvent.dispatch();
        }
    }
};

sgf.sgfLoadingListener = function () {
    sgf.isSGFLoading = true;
};

sgf.sgfLoadedListener = async function () {
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
                await sgf.setRuleset("Japanese");
            } else if (ruleset.includes("chin") || ruleset.includes("korea")) {
                await sgf.setRuleset("Chinese");
            }
        }

        await sgf.setKomi(parseFloat(gameInfo.KM));
    }

    sgf.isSGFLoading = false;
};


sgf.setRuleset = async function (ruleset) {
    sgf.ruleset = ruleset;
    sgf.setRulesetMeta();
    sgf.rulesetElement.innerHTML = ruleset;
    if (trainerG.phase != trainerG.PHASE_TYPE.NONE && trainerG.phase != trainerG.PHASE_TYPE.INIT) await katago.setRuleset();
};

sgf.setKomi = async function (komi) {
    sgf.komi = komi;
    sgf.setKomiMeta();
    sgf.komiElement.innerHTML = komi;
    if (trainerG.phase != trainerG.PHASE_TYPE.NONE && trainerG.phase != trainerG.PHASE_TYPE.INIT) await katago.setKomi();
};


sgf.setPlayersMeta = function () {
    trainerG.board.editor.setGameInfo(sgf.userName ? sgf.userName : "Player", "P" + g.colorNumToName(trainerG.color));
    trainerG.board.editor.setGameInfo("AI", "P" + g.colorNumToName(trainerG.color * -1));
};

sgf.setRankPlayerMeta = function () {
    trainerG.board.editor.setGameInfo(settings.suggestionVisits + "", g.colorNumToName(trainerG.color) + "R");
};

sgf.setRankAIMeta = function () {
    trainerG.board.editor.setGameInfo(settings.opponentVisits + "", g.colorNumToName(trainerG.color * -1) + "R");
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

sgf.setResultMeta = function (result) {
    trainerG.board.editor.setGameInfo(result, "RE");
};

export { sgf };
