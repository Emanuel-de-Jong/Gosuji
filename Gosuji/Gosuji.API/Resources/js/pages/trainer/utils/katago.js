import { Move } from "../classes/Move";
import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { settings } from "./settings";
import { sgf } from "./sgf";
import { trainerG } from "./trainerG";

let katago = { id: "katago" };


katago.init = async function (serviceRef) {
    katago.serviceRef = serviceRef;
    katago.isStarted = false;
};

katago.clear = async function () {
    katago.isStarted = false;
    await katago.start();
};


katago.start = async function () {
    if (katago.isStarted) {
        return;
    }
    katago.isStarted = true;

    let invokeResult = await katago.sendPageRequest("Start");
    if (!invokeResult) {
        return;
    }

    await katago.initTrainerConnection();
};

katago.initTrainerConnection = async function () {
    return await katago.sendPageRequest("InitTrainerConnection", sgf.ruleset, sgf.komi);
};

katago.syncBoard = async function () {
    let moves = trainerG.board.getMoves();
    return await katago.sendRequest("SyncBoard", moves);
};

katago.setRuleset = async function () {
    return await katago.sendRequest("SetRuleset", sgf.ruleset);
};

katago.setKomi = async function () {
    return await katago.sendRequest("SetKomi", sgf.komi);
};

katago.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    let kataGoSuggestion = await katago.sendRequest("AnalyzeMove", new Move(color, coord));
    if (kataGoSuggestion == null) {
        return;
    }

    return MoveSuggestion.fromKataGo(kataGoSuggestion);
};

katago.analyze = async function (
    maxVisits = settings.suggestionVisits,
    moveOptions = settings.suggestionOptions,
    minVisitsPerc = settings.minVisitsPerc,
    maxVisitDiffPerc = settings.maxVisitDiffPerc,
    color = trainerG.board.getNextColor()
) {
    minVisitsPerc = settings.minVisitsPercSwitch ? minVisitsPerc : 0;
    maxVisitDiffPerc = settings.maxVisitDiffPercSwitch ? maxVisitDiffPerc : 100;
    
    let kataGoSuggestions = await katago.sendRequest("Analyze", color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
    if (kataGoSuggestions == null) {
        return;
    }

    let suggestions = MoveSuggestionList.fromKataGo(kataGoSuggestions);
    suggestions.filterByPass();
    suggestions.filterByMoveOptions(moveOptions);
    return suggestions;
};

katago.play = async function (coord, color = trainerG.board.getColor()) {
    return await katago.sendRequest("Play", new Move(color, coord));
};

katago.sendRequest = async function (uri, ...args) {
    trainerG.showLoadAnimation();

    await katago.start();

    let result;
    try {
        result = await katago.serviceRef.invokeMethodAsync(uri, ...args);
    } catch (error) {
        console.error(error);
    }

    trainerG.hideLoadAnimation();
    return result;
};

katago.sendPageRequest = async function (uri, ...args) {
    trainerG.showLoadAnimation();

    let result;
    try {
        result = await trainerG.trainerRef.invokeMethodAsync(uri, ...args);
    } catch (error) {
        console.error(error);
    }

    trainerG.hideLoadAnimation();
    return result;
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.katago) window.trainer.katago = katago;

export { katago };
