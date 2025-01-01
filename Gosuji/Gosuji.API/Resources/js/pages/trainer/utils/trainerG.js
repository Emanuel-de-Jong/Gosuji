import { History } from "../classes/History";
import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { TrainerBoard } from "../classes/TrainerBoard";
import { boardOverlay } from "./boardOverlay";
import { kataGo } from "./kataGo";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let trainerG = { id: "trainerG" };

trainerG.SUGGESTIONS_HISTORY_NAME = "suggestions";
trainerG.MOVE_ORIGIN_HISTORY_NAME = "moveOrigin";

trainerG.MOVE_ORIGIN = {
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


trainerG.init = function (trainerRef) {
    trainerG.trainerRef = trainerRef;

    trainerG.loadAnimation = document.querySelector("#trainerGame .loadAnimation");
    trainerG.phaseChangedEvent = new CEvent();
    trainerG.board = new TrainerBoard();

    trainerG.clear();
};

trainerG.clear = function () {
    trainerG.setPhase(trainerG.PHASE_TYPE.NONE);
    trainerG.setColor(null);
    trainerG.suggestions = null;
    trainerG.suggestionsHistory = new History(trainerG.SUGGESTIONS_HISTORY_NAME, false);
    trainerG.moveOriginHistory = new History(trainerG.MOVE_ORIGIN_HISTORY_NAME, false);
    trainerG.isPassed = false;
};

trainerG.getMainBranch = function () {
    return trainerG.moveOriginHistory.getHighestX();
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

trainerG.setGameLoadInfo = function (gameLoadInfo) {
    if (!gameLoadInfo) {
        return;
    }
    
    gameLoadInfo.moveTree = JSON.parse(gameLoadInfo.moveTree);
    trainerG.gameLoadInfo = gameLoadInfo;
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

trainerG.handleResult = function (result) {
    if (result == null) return;

    trainerG.isPassed = true;
    gameplay.takePlayerControl();
    trainerG.board.nextButton.disabled = true;

    trainerG.board.pass();

    stats.resultHistory.add(result);
    
    let resultStr = g.getResultStr(result);
    stats.setResult(resultStr);
    sgf.setResultMeta(resultStr);

    boardOverlay.finishedOverlay.hidden = false;
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.trainerG) window.trainer.trainerG = trainerG;

export { trainerG };
