import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { settings } from "./settings";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { trainerG } from "./trainerG";
import { gameplay } from "../gameplay";

let kataGo = { id: "kataGo" };


kataGo.init = async function (serviceRef) {
    kataGo.serviceRef = serviceRef;
    kataGo.isStarted = false;
};

kataGo.clear = async function () {
    kataGo.isStarted = false;
    await kataGo.start();
};


kataGo.start = async function () {
    if (kataGo.isStarted) {
        return;
    }
    kataGo.isStarted = true;

    let thirdPartyMoves;
    let name;

    if (sgf.isThirdParty) {
        thirdPartyMoves = trainerG.board.getAllMoves();
        name = "MySGF";
    }

    return await kataGo.sendPageRequest("InitTrainerConnection", sgf.ruleset, sgf.komi,
        thirdPartyMoves, name);
};

kataGo.analyze = async function (
    moveType = trainerG.MOVE_TYPE.PLAYER,
    color = trainerG.board.getNextColor()
) {
    let isMainBranch = trainerG.isMainBranch();

    let analyzeResponse = await kataGo.sendRequest("Analyze", moveType, color, isMainBranch);
    if (analyzeResponse == null) {
        return;
    }

    trainerG.suggestions = MoveSuggestionList.fromKataGo(analyzeResponse.suggestionList);
    trainerG.handleResult(analyzeResponse.result);
    
    return trainerG.suggestions;
};

kataGo.analyzeAfterJump = async function (
    moveType = trainerG.MOVE_TYPE.PLAYER,
    color = trainerG.board.getNextColor()
) {
    let isMainBranch = trainerG.isMainBranch();
    let moves = trainerG.board.getMoves();

    let analyzeResponse = await kataGo.sendRequest("AnalyzeAfterJump", moves, moveType, color, isMainBranch);
    if (analyzeResponse == null) {
        return;
    }

    trainerG.suggestions = MoveSuggestionList.fromKataGo(analyzeResponse.suggestionList);
    trainerG.handleResult(analyzeResponse.result);

    return trainerG.suggestions;
};

kataGo.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    let kataGoSuggestion = await kataGo.sendRequest("AnalyzeMove", new Move(color, coord));
    if (kataGoSuggestion == null) {
        return;
    }

    let suggestion = MoveSuggestion.fromKataGo(kataGoSuggestion);

    if (!trainerG.suggestions) {
        trainerG.suggestions = new MoveSuggestionList();
    }
    trainerG.suggestions.analyzeMoveSuggestion = suggestion;

    return suggestion;
};

kataGo.playPlayer = async function (coord, color = trainerG.board.getColor()) {
    return await kataGo.sendRequest("PlayPlayer",
        new Move(color, coord),
        stats.playerResultHistory.get(),
        gameplay.chosenNotPlayedCoordHistory.get(trainerG.board.get().parent),
        stats.rightStreak,
        stats.perfectStreak,
        stats.rightTopStreak,
        stats.perfectTopStreak
    );
};

kataGo.playForcedCorner = async function (coord, color = trainerG.board.getNextColor()) {
    let kataGoSuggestion = await kataGo.sendRequest("PlayForcedCorner", new Move(color, coord));
    if (kataGoSuggestion == null) {
        return;
    }
    
    let suggestion = MoveSuggestion.fromKataGo(kataGoSuggestion);

    if (!trainerG.suggestions) {
        trainerG.suggestions = new MoveSuggestionList();
    }
    trainerG.suggestions.analyzeMoveSuggestion = suggestion;

    return suggestion;
};

kataGo.sendRequest = async function (uri, ...args) {
    trainerG.showLoadAnimation();

    await kataGo.start();

    let result = await kataGo.invokeCS(kataGo.serviceRef, uri, ...args);

    trainerG.hideLoadAnimation();
    return result;
};

kataGo.sendPageRequest = async function (uri, ...args) {
    trainerG.showLoadAnimation();

    await kataGo.start();

    let result = await kataGo.invokeCS(trainerG.trainerRef, uri, ...args);

    trainerG.hideLoadAnimation();
    return result;
};

kataGo.invokeCS = async function (ref, uri, ...args) {
    let result;
    try {
        result = await ref.invokeMethodAsync(uri, ...args);
    } catch (error) {
        console.error(error);
    }
    return result;
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.kataGo) window.trainer.kataGo = kataGo;

export { kataGo };
