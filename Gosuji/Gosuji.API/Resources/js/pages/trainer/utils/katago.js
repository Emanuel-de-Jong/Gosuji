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

    let startResult = await trainerG.trainerRef
        .invokeMethodAsync("Start")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    
    if (startResult == false) {
        return;
    }

    await katago.restart();
    await katago.setBoardsize();
    await katago.setRuleset();
    await katago.setKomi();
    await katago.setHandicap();
};

katago.clearBoard = async function () {
    return await katago.sendRequest("ClearBoard");
};

katago.restart = async function () {
    return await katago.sendRequest("Restart");
};

katago.setBoardsize = async function () {
    return await katago.sendRequest("SetBoardsize", trainerG.board.boardsize);
};

katago.setRuleset = async function () {
    return await katago.sendRequest("SetRuleset", sgf.ruleset);
};

katago.setKomi = async function () {
    return await katago.sendRequest("SetKomi", sgf.komi);
};

katago.setHandicap = async function () {
    if (!trainerG.board.handicap) return;

    return await katago.sendRequest("SetHandicap", trainerG.board.handicap);
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
    suggestions.addGrades();
    return suggestions;
};

katago.play = async function (coord, color = trainerG.board.getColor()) {
    return await katago.sendRequest("Play", new Move(color, coord));
};

katago.playRange = async function () {
    let moves = trainerG.board.getMoves();
    if (moves.length == 0) return;

    let serverMoves = {
        moves: moves,
    };

    return await katago.sendRequest("PlayRange", serverMoves);
};

katago.sgf = async function () {
    return await katago.sendRequest("SGF", false);
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

export { katago };
