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

    await katago.restart();
    await katago.setBoardsize();
    await katago.setRuleset();
    await katago.setKomi();
    await katago.setHandicap();
};

katago.clearBoard = async function () {
    await katago.start();

    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("ClearBoard")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.restart = async function () {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("Restart")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.setBoardsize = async function () {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("SetBoardsize", trainerG.board.boardsize)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.setRuleset = async function () {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("SetRuleset", sgf.ruleset)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.setKomi = async function () {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("SetKomi", sgf.komi)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.setHandicap = async function () {
    if (!trainerG.board.handicap) return;

    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("SetHandicap", trainerG.board.handicap)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("AnalyzeMove", g.colorNumToName(color), katago.coordNumToName(coord))
        .then((kataGoSuggestion) => {
            return MoveSuggestion.fromKataGo(kataGoSuggestion);
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
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
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("Analyze", g.colorNumToName(color), maxVisits, minVisitsPerc, maxVisitDiffPerc)
        .then((kataGoSuggestions) => {
            let suggestions = MoveSuggestionList.fromKataGo(kataGoSuggestions);
            suggestions.filterByPass();
            suggestions.filterByMoveOptions(moveOptions);
            suggestions.addGrades();
            return suggestions;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.play = async function (coord, color = trainerG.board.getColor()) {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("Play", g.colorNumToName(color), katago.coordNumToName(coord))
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.playRange = async function () {
    let moves = trainerG.board.getMoves();
    if (moves.length == 0) return;

    let serverMoves = {
        moves: [],
    };

    for (let i = 0; i < moves.length; i++) {
        let move = moves[i];
        serverMoves.moves.push({
            color: g.colorNumToName(move.color),
            coord: katago.coordNumToName(move.coord),
        });
    }

    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("PlayRange", serverMoves)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.sgf = async function () {
    await katago.start();
    
    trainerG.showLoadAnimation();
    let response = await katago.serviceRef
        .invokeMethodAsync("SGF", false)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
    trainerG.hideLoadAnimation();
    return response;
};

katago.sendRequest = async function (request) {
    // Before
    let response = await request;
    // After
    return response;
};

katago.coordNumToName = function (numCoord) {
    if (numCoord.x == 0) return "pass";

    let xConvert = {
        1: "A",
        2: "B",
        3: "C",
        4: "D",
        5: "E",
        6: "F",
        7: "G",
        8: "H",
        9: "J",
        10: "K",
        11: "L",
        12: "M",
        13: "N",
        14: "O",
        15: "P",
        16: "Q",
        17: "R",
        18: "S",
        19: "T",
    };

    let x = xConvert[numCoord.x];
    let y = trainerG.board.boardsize + 1 - numCoord.y;
    return "" + x + y;
};

katago.coordNameToNum = function (nameCoord) {
    if (nameCoord == "pass") return new Coord(0, 0);

    let xConvert = {
        A: 1,
        B: 2,
        C: 3,
        D: 4,
        E: 5,
        F: 6,
        G: 7,
        H: 8,
        J: 9,
        K: 10,
        L: 11,
        M: 12,
        N: 13,
        O: 14,
        P: 15,
        Q: 16,
        R: 17,
        S: 18,
        T: 19,
    };

    let nums = nameCoord.substring(1).split(" ");

    let x = xConvert[nameCoord[0]];
    let y = trainerG.board.boardsize + 1 - parseInt(nums[0]);
    return new Coord(x, y);
};

export { katago };
