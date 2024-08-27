import { CNode } from "../classes/CNode";
import { History } from "../classes/History";
import { Ratio } from "../classes/Ratio";
import { settings } from "./settings";
import { trainerG } from "./trainerG";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let stats = { id: "stats" };


stats.PLAYER_RESULT_TYPE = {
    NONE: 0,
    WRONG: 1,
    RIGHT: 2,
    PERFECT: 3,
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

    await stats.clearSuggestions();
    stats.clearResult();

    stats.rightStreak = gameLoadInfo ? gameLoadInfo.rightStreak : 0;
    stats.perfectStreak = gameLoadInfo ? gameLoadInfo.perfectStreak : 0;
    stats.rightTopStreak = gameLoadInfo ? gameLoadInfo.rightTopStreak : 0;
    stats.perfectTopStreak = gameLoadInfo ? gameLoadInfo.perfectTopStreak : 0;

    trainerG.board.editor.addListener(stats.drawStats);

    if (debug.testData == 1) {
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.WRONG, 1, 0);
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.RIGHT, 3, 0);
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.PERFECT, 5, 0);

        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.RIGHT, 1, 1);
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.PERFECT, 3, 1);
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.WRONG, 5, 1);
        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.WRONG, 7, 1);

        stats.playerResultHistory.add(stats.PLAYER_RESULT_TYPE.RIGHT, 5, 2);
    } else if (debug.testData == 2) {
        for (let i = utils.randomInt(61, 1); i < utils.randomInt(241, 80); i += 2) {
            stats.playerResultHistory.add(utils.randomInt(4, 1), i, 0);
        }
    }
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

stats.encodePlayerResultHistory = function () {
    let encoded = stats.encodePlayerResultHistoryLoop();

    let firstY = byteUtils.numToBytes(stats.PLAYER_RESULT_Y_INDICATOR, 2);
    firstY = byteUtils.numToBytes(0, 2, firstY);
    firstY = byteUtils.numToBytes(0, 2, firstY);
    firstY = byteUtils.numToBytes(0, 2, firstY);

    encoded = firstY.concat(encoded);

    // stats.decodePlayerResultHistory(encoded);

    return encoded;
};

stats.encodePlayerResultHistoryLoop = function (node = trainerG.board.editor.getRoot()) {
    let encoded = [];

    for (let i = 0; i < node.children.length; i++) {
        let childNode = node.children[i];

        if (!stats.playerResultHistory.hasY(childNode.navTreeY)) continue;

        if (i > 0) {
            let parentNode = node;
            while (parentNode.parent != null) {
                if (stats.playerResultHistory.get(parentNode.navTreeX, parentNode.navTreeY)) {
                    break;
                }

                parentNode = parentNode.parent;
            }

            encoded = byteUtils.numToBytes(stats.PLAYER_RESULT_Y_INDICATOR, 2, encoded);
            encoded = byteUtils.numToBytes(childNode.navTreeY, 2, encoded);
            encoded = byteUtils.numToBytes(parentNode.navTreeY, 2, encoded);
            encoded = byteUtils.numToBytes(parentNode.navTreeX, 2, encoded);
        }

        let val = stats.playerResultHistory.get(childNode.navTreeX, childNode.navTreeY);
        if (val) {
            encoded = byteUtils.numToBytes(childNode.navTreeX, 2, encoded);
            encoded = byteUtils.numToBytes(val, 1, encoded);
            // console.log(childNode.navTreeY + ", " + childNode.navTreeX + ": " + val);
        }

        encoded = encoded.concat(stats.encodePlayerResultHistoryLoop(childNode));
    }

    return encoded;
};

stats.decodePlayerResultHistory = function (encoded) {
    let rootNode = new CNode();

    let i = 0;
    let y = 0;
    let node = rootNode;
    while (i < encoded.length) {
        let x = encoded[i];
        if (x == stats.PLAYER_RESULT_Y_INDICATOR) {
            y = encoded[i + 1];
            node = rootNode.nodes.get(encoded[i + 3], encoded[i + 2]);
            i += 4;
        } else {
            node = node.add(encoded[i + 1], x, y);
            i += 2;
        }
    }

    stats.printDecodedPlayerResultHistory(rootNode);
};

stats.printDecodedPlayerResultHistory = function (node) {
    for (let i = 0; i < node.children.length; i++) {
        let childNode = node.children[i];
        // console.log(childNode.y + ", " + childNode.x + ": " + childNode.value);
        stats.printDecodedPlayerResultHistory(childNode);
    }
};

stats.getMostPlayerResultsBranch = function (node = trainerG.board.editor.getRoot(), playerResultCount = 0) {
    if (stats.playerResultHistory.get(node.navTreeX, node.navTreeY)) playerResultCount++;

    if (node.children.length == 0) {
        return {
            node: node,
            count: playerResultCount,
        };
    }

    let childPlayerResultCounts = [];
    for (let i = 0; i < node.children.length; i++) {
        childPlayerResultCounts.push(stats.getMostPlayerResultsBranch(node.children[i], playerResultCount));
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

    await trainerG.trainerRef.invokeMethodAsync("SetSuggestions", suggestionPerGrade);
};

stats.clearSuggestions = async function () {
    await trainerG.trainerRef.invokeMethodAsync("SetSuggestions", null);
};


stats.setResult = function (result) {
    stats.resultElement.textContent = result;
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
    
    let node = trainerG.board.editor.getCurrent();
    if (node.parent == null || node.navTreeY == node.parent.navTreeY) {
        stats.setStreaks();
    } else {
        stats.clearStreaks();
    }

    if (trainerG.phase == trainerG.PHASE_TYPE.GAMEPLAY && (
            !event.treeChange ||
            (gameplay.shouldShowPlayerOptions() && trainerG.color == trainerG.board.getColor()) ||
            (settings.showOpponentOptions && trainerG.color != trainerG.board.getColor())
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

export { stats };
