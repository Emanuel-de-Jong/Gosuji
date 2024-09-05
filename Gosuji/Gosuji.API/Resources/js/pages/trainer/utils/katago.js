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
    await katago.start();

    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("ClearBoard")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.restart = async function () {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("Restart")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.setBoardsize = async function () {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("SetBoardsize", trainerG.board.boardsize)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.setRuleset = async function () {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("SetRuleset", sgf.ruleset)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.setKomi = async function () {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("SetKomi", sgf.komi)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.setHandicap = async function () {
    if (!trainerG.board.handicap) return;

    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("SetHandicap", trainerG.board.handicap)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("AnalyzeMove", new Move(color, coord))
        .then((kataGoSuggestion) => {
            return MoveSuggestion.fromKataGo(kataGoSuggestion);
        })
        .catch((error) => {
            return error;
        }));
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
    
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("Analyze", color, maxVisits, minVisitsPerc, maxVisitDiffPerc)
        .then((kataGoSuggestions) => {
            let suggestions = MoveSuggestionList.fromKataGo(kataGoSuggestions);
            suggestions.filterByPass();
            suggestions.filterByMoveOptions(moveOptions);
            suggestions.addGrades();
            return suggestions;
        })
        .catch((error) => {
            return error;
        }));
};

katago.play = async function (coord, color = trainerG.board.getColor()) {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("Play", new Move(color, coord))
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.playRange = async function () {
    let moves = trainerG.board.getMoves();
    if (moves.length == 0) return;

    let serverMoves = {
        moves: moves,
    };

    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("PlayRange", serverMoves)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.sgf = async function () {
    await katago.start();
    
    return katago.sendRequest(katago.serviceRef
        .invokeMethodAsync("SGF", false)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        }));
};

katago.sendRequest = async function (request) {
    trainerG.showLoadAnimation();
    let response = await request;
    trainerG.hideLoadAnimation();
    return response;
};

export { katago };
