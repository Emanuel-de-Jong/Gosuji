import { History } from "../classes/History";
import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { TrainerBoard } from "../classes/TrainerBoard";
import { kataGo } from "./kataGo";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let trainerG = { id: "trainerG" };


trainerG.MOVE_TYPE = {
    INIT: 0,
    FORCED_CORNER: 1,
    PRE: 2,
    SELFPLAY: 3,
    PLAYER: 4,
    OPPONENT: 5,
    PASS: 6,
};

trainerG.PHASE_TYPE = {
    NONE: 0,
    INIT: 1,
    RESTART: 2,
    CORNERS: 3,
    PREMOVES: 4,
    GAMEPLAY: 5,
    SELFPLAY: 6,
    FINISHED: 7,
};


trainerG.isLoadingServerData = false;


trainerG.init = function (trainerRef, gameLoadInfo) {
    trainerG.trainerRef = trainerRef;

    trainerG.loadAnimation = document.querySelector("#trainerGame .loadAnimation");
    trainerG.phaseChangedEvent = new CEvent();
    trainerG.board = new TrainerBoard();

    trainerG.clear(gameLoadInfo);
};

trainerG.clear = function (gameLoadInfo) {
    trainerG.setPhase(trainerG.PHASE_TYPE.NONE);
    trainerG.setColor(null);
    trainerG.suggestions = null;
    trainerG.suggestionsHistory = gameLoadInfo
        ? History.fromServer(gameLoadInfo.suggestions, MoveSuggestionList)
        : new History();
    trainerG.moveTypeHistory = gameLoadInfo ? History.fromServer(gameLoadInfo.moveTypes) : new History();
    trainerG.result = null;
    trainerG.isPassed = false;
};

trainerG.getMainBranch = function () {
    return trainerG.moveTypeHistory.getHighestX();
};

trainerG.isMainBranch = function () {
    let isMainBranch = false;
    const mainBranchNode = trainerG.getMainBranch();
    const currentNode = trainerG.board.get();
    if (mainBranchNode != null && (
        mainBranchNode.navTreeX != currentNode.navTreeX ||
        mainBranchNode.navTreeY != currentNode.navTreeY)
    ) {
        isMainBranch = true;
    }

    return isMainBranch;
};

trainerG.getGameColor = function () {
    let color = trainerG.color;

    let node = trainerG.getMainBranch();
    if (node == null) {
        return color;
    }

    let moveType;
    while (node) {
        const tempMoveType = trainerG.moveTypeHistory.get(node);
        if (tempMoveType != null && tempMoveType == trainerG.MOVE_TYPE.PLAYER) {
            moveType = tempMoveType;
            break;
        }

        node = node.parent;
    }
    if (moveType == null) {
        return color;
    }

    color = node.move.color;
    return color;
};

trainerG.setPhase = function (phase) {
    trainerG.phase = phase;
    trainerG.phaseChangedEvent.dispatch({ phase: phase });
};

trainerG.setColor = function (color = trainerG.board.getNextColor()) {
    if (color == g.COLOR_TYPE.RANDOM) {
        color = utils.randomInt(2) == 0 ? g.COLOR_TYPE.B : g.COLOR_TYPE.W;
    }

    trainerG.color = color;
    if (trainerG.phase != trainerG.PHASE_TYPE.NONE &&
        trainerG.phase != trainerG.PHASE_TYPE.INIT &&
        trainerG.phase != trainerG.PHASE_TYPE.RESTART) {
        sgf.setPlayersMeta();
    }
};


trainerG.showLoadAnimation = function() {
    trainerG.loadAnimation.hidden = false;
};

trainerG.hideLoadAnimation = function() {
    trainerG.loadAnimation.hidden = true;
};

trainerG.analyze = async function (moveType, color) {
    trainerG.suggestions = await kataGo.analyze(moveType, color);
    await trainerG.pass(trainerG.suggestions.passSuggestion);
};

trainerG.analyzeAfterJump = async function (moveType, color) {
    trainerG.suggestions = await kataGo.analyzeAfterJump(moveType, color);
    await trainerG.pass(trainerG.suggestions.passSuggestion);
};

trainerG.analyzeMove = async function (coord) {
    let suggestion = await kataGo.analyzeMove(coord);

    if (!trainerG.suggestions) {
        trainerG.suggestions = new MoveSuggestionList();
    }
    trainerG.suggestions.analyzeMoveSuggestion = suggestion;

    return suggestion;
};

trainerG.pass = async function (suggestion) {
    if (!suggestion) return;

    trainerG.isPassed = true;
    gameplay.takePlayerControl();
    trainerG.board.nextButton.disabled = true;

    trainerG.board.pass();

    if (!trainerG.isMainBranch()) {
        return;
    }

    trainerG.result = suggestion.score.copy();

    let resultStr = trainerG.getResultStr();
    stats.setResult(resultStr);
    sgf.setResultMeta(resultStr);

    trainerG.board.finishedOverlay.hidden = false;
};

trainerG.getResultStr = function () {
    if (trainerG.result.scoreLead >= 0) {
        return g.COLOR_NAME_TYPE.B + "+" + trainerG.result.formatScoreLead();
    }
    return g.COLOR_NAME_TYPE.W + "+" + trainerG.result.formatScoreLead(true);
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.trainerG) window.trainer.trainerG = trainerG;

export { trainerG };
