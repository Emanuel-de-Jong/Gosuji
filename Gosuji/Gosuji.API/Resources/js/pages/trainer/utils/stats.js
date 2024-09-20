import { History } from "../classes/History";
import { settings } from "./settings";
import { trainerG } from "./trainerG";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let stats = { id: "stats" };


stats.PLAYER_RESULT_TYPE = {
    WRONG: 0,
    RIGHT: 1,
    PERFECT: 2,
};

stats.PLAYER_RESULT_Y_INDICATOR = -1;


stats.init = async function (gameLoadInfo) {
    stats.rightStreakElement = document.getElementById("rightStreak");
    stats.rightTopStreakElement = document.getElementById("rightTopStreak");
    stats.perfectStreakElement = document.getElementById("perfectStreak");
    stats.perfectTopStreakElement = document.getElementById("perfectTopStreak");

    stats.resultDivElement = document.getElementById("resultDiv");
    stats.resultElement = document.getElementById("result");

    await stats.clear(gameLoadInfo);
};

stats.clear = async function (gameLoadInfo) {
    stats.playerResultHistory = gameLoadInfo ? History.fromServer(gameLoadInfo.playerResults) : new History();
    stats.resultHistory = new History();

    await stats.clearSuggestions();
    stats.clearResult();

    stats.rightStreak = gameLoadInfo ? gameLoadInfo.rightStreak : 0;
    stats.perfectStreak = gameLoadInfo ? gameLoadInfo.perfectStreak : 0;
    stats.rightTopStreak = gameLoadInfo ? gameLoadInfo.rightTopStreak : 0;
    stats.perfectTopStreak = gameLoadInfo ? gameLoadInfo.perfectTopStreak : 0;

    trainerG.board.editor.addListener(stats.drawStats);
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


stats.setResult = function (result = stats.resultHistory.findFirstInBranch()) {
    if (result == null) {
        return;
    }

    let resultStr = g.getResultStr(result);
    stats.resultElement.textContent = resultStr;
    stats.resultDivElement.hidden = false;
};

stats.clearResult = function () {
    stats.resultElement.textContent = "";
    stats.resultDivElement.hidden = true;
};


stats.drawStats = async function (event) {
    if (!event.navChange) {
        return;
    }
    
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
        if (!event.treeChange || trainerG.board.getNodeX() == 0) trainerG.suggestions = trainerG.suggestionsHistory.get();

        if (trainerG.suggestions) {
            await stats.setSuggestions(trainerG.suggestions);
            trainerG.board.drawCoords(trainerG.suggestions);
        }
    } else {
        await stats.clearSuggestions();
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.stats) window.trainer.stats = stats;

export { stats };
