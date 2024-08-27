import { History } from "../classes/History";
import { MoveSuggestionList } from "../classes/MoveSuggestionList";
import { MoveSuggestion } from "../classes/MoveSuggestion";
import { TrainerBoard } from "../classes/TrainerBoard";
import { katago } from "./katago";
import { sgf } from "./sgf";
import { stats } from "./stats";
import { debug } from "../debug";
import { gameplay } from "../gameplay";

let trainerG = { id: "trainerG" };


trainerG.MOVE_TYPE = {
    NONE: 0,
    INIT: 1,
    FORCED_CORNER: 2,
    PRE: 3,
    SELFPLAY: 4,
    PLAYER: 5,
    OPPONENT: 6,
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
    trainerG.wasPassed = false;
    trainerG.shouldBeImperfectSuggestion = false;

    if (debug.testData == 1) {
        trainerG.suggestionsHistory.add(
            new MoveSuggestionList(
                [
                    new MoveSuggestion(new Coord(16, 4), 700, 50_000_000, 800_000),
                    new MoveSuggestion(new Coord(17, 4), 500, 49_000_000, -200_000),
                ],
                new MoveSuggestion(new Coord(12, 12), 250, 20_000_000, -6_000_000)
            ).addGrades(),
            1,
            0
        );
        trainerG.suggestionsHistory.add(
            new MoveSuggestionList([
                new MoveSuggestion(new Coord(16, 3), 600, 51_000_000, 1_200_000),
                new MoveSuggestion(new Coord(17, 4), 600, 51_000_000, 1_200_000),
            ]).addGrades(),
            3,
            0
        );
    }

    if (debug.testData == 1) {
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 1, 0);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.OPPONENT, 2, 0);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 3, 0);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.OPPONENT, 4, 0);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 5, 0);

        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 1, 1);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 3, 1);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.OPPONENT, 4, 1);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 5, 1);

        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.PLAYER, 5, 2);
        trainerG.moveTypeHistory.add(trainerG.MOVE_TYPE.OPPONENT, 6, 2);
    }
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

trainerG.setKataGoVersion = function (kataGoVersion) {
    trainerG.kataGoVersion = kataGoVersion;
};


trainerG.showLoadAnimation = function() {
    trainerG.loadAnimation.hidden = false;
};

trainerG.hideLoadAnimation = function() {
    trainerG.loadAnimation.hidden = true;
};

trainerG.analyze = async function (maxVisits, moveOptions, minVisitsPerc, maxVisitDiffPerc, color) {
    trainerG.suggestions = await katago.analyze(maxVisits, moveOptions, minVisitsPerc, maxVisitDiffPerc, color);
    await trainerG.pass(trainerG.suggestions.passSuggestion);
};

trainerG.analyzeMove = async function (coord) {
    let suggestion = await katago.analyzeMove(coord);

    if (!trainerG.suggestions) {
        trainerG.suggestions = new MoveSuggestionList();
    }
    trainerG.suggestions.analyzeMoveSuggestion = suggestion;

    return suggestion;
};

trainerG.pass = async function (suggestion) {
    if (!suggestion) return;

    trainerG.isPassed = true;
    trainerG.wasPassed = true;
    gameplay.takePlayerControl();
    trainerG.board.nextButton.disabled = true;

    trainerG.result = suggestion.score.copy();

    let resultStr = trainerG.getResultStr();
    stats.setResult(resultStr);
    sgf.setResultMeta(resultStr);

    trainerG.board.pass();

    trainerG.board.finishedOverlay.hidden = false;

    await db.save();
};

trainerG.getResultStr = function () {
    if (trainerG.result.scoreLead >= 0) {
        return g.COLOR_NAME_TYPE.B + "+" + trainerG.result.formatScoreLead();
    }
    return g.COLOR_NAME_TYPE.W + "+" + trainerG.result.formatScoreLead(true);
};

export { trainerG };
