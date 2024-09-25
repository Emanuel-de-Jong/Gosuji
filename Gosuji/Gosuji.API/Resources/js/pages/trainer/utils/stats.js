import { History } from "../classes/History";
import { settings } from "./settings";
import { trainerG } from "./trainerG";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let stats = { id: "stats" };

stats.PLAYER_RESULT_HISTORY_NAME = "playerResult";
stats.RESULT_HISTORY_NAME = "result";

stats.PLAYER_RESULT_TYPE = {
    WRONG: 0,
    RIGHT: 1,
    PERFECT: 2,
};

stats.PLAYER_RESULT_Y_INDICATOR = -1;


stats.init = async function () {
    stats.rightStreakElement = document.getElementById("rightStreak");
    stats.rightTopStreakElement = document.getElementById("rightTopStreak");
    stats.perfectStreakElement = document.getElementById("perfectStreak");
    stats.perfectTopStreakElement = document.getElementById("perfectTopStreak");

    stats.resultDivElement = document.getElementById("resultDiv");
    stats.resultElement = document.getElementById("result");

    trainerG.board.addListener(stats.drawStats);

    await stats.clear();
};

stats.clear = async function () {
    stats.playerResultHistory = new History(stats.PLAYER_RESULT_HISTORY_NAME);
    stats.resultHistory = new History(stats.RESULT_HISTORY_NAME);

    await stats.clearSuggestions();
    stats.clearResult();

    stats.rightStreak = trainerG.gameLoadInfo ? trainerG.gameLoadInfo.rightStreak : 0;
    stats.perfectStreak = trainerG.gameLoadInfo ? trainerG.gameLoadInfo.perfectStreak : 0;
    stats.rightTopStreak = trainerG.gameLoadInfo ? trainerG.gameLoadInfo.rightTopStreak : 0;
    stats.perfectTopStreak = trainerG.gameLoadInfo ? trainerG.gameLoadInfo.perfectTopStreak : 0;
};


stats.update = function () {
    let type = stats.PLAYER_RESULT_TYPE.WRONG;
    if (trainerG.isPerfectChoice) {
        type = stats.PLAYER_RESULT_TYPE.PERFECT;
    } else if (trainerG.isRightChoice) {
        type = stats.PLAYER_RESULT_TYPE.RIGHT;
    }

    stats.playerResultHistory.add(type);

    stats.updateStreaks(type);
};

stats.getMostPlayerResultsBranch = function (node = trainerG.board.editor.getRoot(), playerResultCount = 0) {
    if (stats.playerResultHistory.has(node)) playerResultCount++;

    if (node.children.length == 0) {
        return {
            node: node,
            count: playerResultCount,
        };
    }

    let childPlayerResultCounts = [];
    for (const child of node.children) {
        childPlayerResultCounts.push(stats.getMostPlayerResultsBranch(child, playerResultCount));
    }

    childPlayerResultCounts.sort((a, b) => {
        return b.count - a.count;
    });
    return childPlayerResultCounts[0];
};

stats.updateStreaks = function (type) {
    if (type == stats.PLAYER_RESULT_TYPE.PERFECT || type == stats.PLAYER_RESULT_TYPE.RIGHT) {
        stats.rightStreak++;
        if (stats.rightTopStreak < stats.rightStreak) {
            stats.rightTopStreak = stats.rightStreak;
        }
    } else {
        stats.rightStreak = 0;
    }

    if (type == stats.PLAYER_RESULT_TYPE.PERFECT) {
        stats.perfectStreak++;
        if (stats.perfectTopStreak < stats.perfectStreak) {
            stats.perfectTopStreak = stats.perfectStreak;
        }
    } else {
        stats.perfectStreak = 0;
    }
};

stats.setStreaks = function () {
    stats.rightStreakElement.textContent = stats.rightStreak;
    stats.rightTopStreakElement.textContent = stats.rightTopStreak;
    stats.perfectStreakElement.textContent = stats.perfectStreak;
    stats.perfectTopStreakElement.textContent = stats.perfectTopStreak;
};

stats.clearStreaks = function () {
    stats.rightStreak = 0;
    stats.perfectStreak = 0;

    stats.rightStreakElement.textContent = 0;
    stats.perfectStreakElement.textContent = 0;
};


stats.setSuggestions = async function (suggestionList) {
    let suggestions = suggestionList.getFilterByWeaker();
    let suggestionPerGrade = [];
    for (let i = 0; i < suggestions.length; i++) {
        let suggestion = suggestions[i];
        if (i != 0 && suggestion.visits == suggestions[i - 1].visits) continue;
        
        suggestionPerGrade.push(suggestion);
    }

    if (suggestionList.analyzeMoveSuggestion) {
        suggestionPerGrade.push(suggestionList.analyzeMoveSuggestion);
    }

    await trainerG.trainerRef.invokeMethodAsync("SetSuggestions", suggestionPerGrade);
};

stats.clearSuggestions = async function () {
    await trainerG.trainerRef.invokeMethodAsync("SetSuggestions", null);
};


stats.setResult = function (resultStr) {
    if (resultStr == null) {
        let result = stats.resultHistory.findFirstInBranch();
        if (result == null) {
            stats.clearResult();
            return;
        }

        resultStr = g.getResultStr(result);
    }

    stats.resultElement.textContent = resultStr;
    stats.resultDivElement.hidden = false;
};

stats.clearResult = function () {
    stats.resultDivElement.hidden = true;
    stats.resultElement.textContent = "";
};


stats.drawStats = async function (event) {
    console.log("sdofh");

    if (!event.navChange) {
        return;
    }

    trainerG.board.clearMarkups();
    
    let node = trainerG.board.get();
    if (node.parent == null || node.navTreeY == node.parent.navTreeY) {
        stats.setStreaks();
    } else {
        stats.clearStreaks();
    }

    if (trainerG.phase == trainerG.PHASE_TYPE.GAMEPLAY && (
            !event.treeChange ||
            (gameplay.shouldShowPlayerOptions() && trainerG.color == trainerG.board.getColor()) ||
            (gameplay.shouldShowOpponentOptions() && trainerG.color != trainerG.board.getColor())
        )
    ) {
        let chosenNotPlayedCoord;
        if (!event.treeChange || trainerG.board.getNodeX() == 0) {
            trainerG.suggestions = trainerG.suggestionsHistory.get();
            chosenNotPlayedCoord = gameplay.chosenNotPlayedCoordHistory.get();
        }

        if (trainerG.suggestions) {
            await stats.setSuggestions(trainerG.suggestions);
            trainerG.board.drawSuggestionList(trainerG.suggestions);
        }

        if (chosenNotPlayedCoord) {
            trainerG.board.get().addMarkup(chosenNotPlayedCoord.x, chosenNotPlayedCoord.y, 4);
        }

        trainerG.board.redrawMarkup();
    } else {
        await stats.clearSuggestions();
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.stats) window.trainer.stats = stats;

export { stats };
