var katago = {};


katago.init = async function () {
    await katago.clear();
};

katago.clear = async function () {
    await katago.restart();
    await katago.setBoardsize();
    await katago.setRuleset();
    await katago.setKomi();
    await katago.setHandicap();
};


katago.clearBoard = async function () {
    if (G.LOG) console.log("katago.clearBoard");

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("ClearBoard")
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.restart = async function () {
    if (G.LOG) console.log("katago.restart");

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("Restart")
        .then((response) => {
            if (response == false) {
                alert("You already use this page somewhere else!");
            }

            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.setBoardsize = async function () {
    if (G.LOG) console.log("katago.setBoardsize " + trainerG.board.boardsize);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("SetBoardsize", trainerG.board.boardsize)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.setRuleset = async function () {
    if (G.LOG) console.log("katago.setRuleset " + sgf.ruleset);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("SetRuleset", sgf.ruleset)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.setKomi = async function () {
    if (G.LOG) console.log("katago.setKomi " + sgf.komi);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("SetKomi", sgf.komi)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.setHandicap = async function () {
    if (!trainerG.board.handicap) return;

    if (G.LOG) console.log("katago.setHandicap " + trainerG.board.handicap);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("SetHandicap", trainerG.board.handicap)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.analyzeMove = async function (coord, color = trainerG.board.getNextColor()) {
    if (G.LOG) console.log("katago.analyzeMove " + G.colorNumToName(color) + " " + katago.coordNumToName(coord));

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("AnalyzeMove", G.colorNumToName(color), katago.coordNumToName(coord))
        .then((kataGoSuggestion) => {
            return MoveSuggestion.fromKataGo(kataGoSuggestion);
        })
        .catch((error) => {
            return error;
        });
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

    if (G.LOG)
        console.log(
            "katago.analyze " +
                maxVisits + " " +
                moveOptions + " " +
                minVisitsPerc + " " +
                maxVisitDiffPerc + " " +
                color
        );

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("Analyze", G.colorNumToName(color), maxVisits, minVisitsPerc, maxVisitDiffPerc)
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
};

katago.play = async function (coord, color = trainerG.board.getColor()) {
    if (G.LOG) console.log("katago.play " + G.colorNumToName(color) + " " + katago.coordNumToName(coord));

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("Play", G.colorNumToName(color), katago.coordNumToName(coord))
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
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
            color: G.colorNumToName(move.color),
            coord: katago.coordNumToName(move.coord),
        });
    }

    if (G.LOG) console.log("katago.playRange " + serverMoves);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("PlayRange", serverMoves)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
};

katago.sgf = async function () {
    if (G.LOG) console.log("katago.sgf " + false);

    return trainerG.cKataGoWrapperRef
        .invokeMethodAsync("SGF", false)
        .then((response) => {
            return response;
        })
        .catch((error) => {
            return error;
        });
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
