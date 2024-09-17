import { Move } from "../classes/Move";
import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { settings } from "./settings";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { trainerG } from "./trainerG";

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

    let invokeResult = await kataGo.sendPageRequest("Start");
    if (!invokeResult) {
        return;
    }

    await kataGo.initTrainerConnection();
};

kataGo.initTrainerConnection = async function () {
    return await kataGo.sendPageRequest("InitTrainerConnection", sgf.ruleset, sgf.komi, sgf.isThirdParty);
};

kataGo.syncBoard = async function () {
    let moves = trainerG.board.getMoves();
    return await kataGo.sendRequest("SyncBoard", moves);
};

kataGo.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    let kataGoSuggestion = await kataGo.sendRequest("AnalyzeMove", new Move(color, coord));
    if (kataGoSuggestion == null) {
        return;
    }

    return MoveSuggestion.fromKataGo(kataGoSuggestion);
};

kataGo.analyze = async function (
    moveType = trainerG.MOVE_TYPE.PLAYER,
    color = trainerG.board.getNextColor()
) {
    let kataGoSuggestions = await kataGo.sendRequest("Analyze", moveType, color);
    if (kataGoSuggestions == null) {
        return;
    }

    return MoveSuggestionList.fromKataGo(kataGoSuggestions);
};

kataGo.playPlayer = async function (coord, color = trainerG.board.getColor()) {
    return await kataGo.sendRequest("PlayPlayer",
        new Move(color, coord),
        stats.playerResultHistory.get(),
        stats.rightStreak,
        stats.perfectStreak,
        stats.rightTopStreak,
        stats.perfectTopStreak
    );
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
