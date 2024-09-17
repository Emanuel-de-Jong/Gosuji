import { History } from "./classes/History";
import { ratioChart } from "./utils/ratioChart";
import { scoreChart } from "./utils/scoreChart";
import { settings } from "./utils/settings";
import { sgf } from "./utils/sgf";
import { stats } from "./utils/stats";
import { trainerG } from "./utils/trainerG";
import { cornerPlacer } from "./cornerPlacer";
import { selfplay } from "./selfplay";
import { debug } from "./debug";
import { kataGo } from "./utils/kataGo";

let gameplay = { id: "gameplay" };


gameplay.OPPONENT_MIN_VISITS_PERC = 10;
gameplay.OPPONENT_MAX_VISIT_DIFF_PERC = 50;


gameplay.init = function (gameLoadInfo) {
    gameplay.clear(gameLoadInfo);
};

gameplay.clear = function (gameLoadInfo) {
    gameplay.suggestionsPromise = null;
    gameplay.chosenNotPlayedCoordHistory = gameLoadInfo
        ? History.fromServer(gameLoadInfo.chosenNotPlayedCoords, Coord)
        : new History();
    gameplay.isPlayerControlling = false;
    gameplay.isJumped = false;
    gameplay.playerTurnId = 0;
    gameplay.opponentTurnId = 0;

    trainerG.board.editor.addListener(gameplay.playerMarkupPlacedCheckListener);
    trainerG.board.editor.addListener(gameplay.detectJump);
    trainerG.board.nextButton.addEventListener("click", gameplay.nextButtonClickListener);
};


gameplay.start = function (isSuggestionNeeded = true) {
    trainerG.setPhase(trainerG.PHASE_TYPE.GAMEPLAY);

    if (trainerG.color == trainerG.board.getColor()) {
        if (!cornerPlacer.shouldForce()) {
            gameplay.suggestionsPromise = trainerG.analyze(trainerG.MOVE_TYPE.OPPONENT);
        }
        gameplay.opponentTurn();
    } else {
        gameplay.givePlayerControl(isSuggestionNeeded);
    }
};

gameplay.givePlayerControl = function (isSuggestionNeeded = true) {
    trainerG.board.editor.setTool("cross");
    gameplay.isPlayerControlling = true;
    if (isSuggestionNeeded) {
        gameplay.suggestionsPromise = trainerG.analyze();
    }
};

gameplay.takePlayerControl = function () {
    trainerG.board.editor.setTool("navOnly");
    gameplay.isPlayerControlling = false;
};

gameplay.playerMarkupPlacedCheckListener = async function (event) {
    if (event.markupChange && event.mark == 4 && gameplay.isPlayerControlling && !sgf.isSGFLoading) {
        let markupCoord = new Coord(event.x, event.y);

        // Coord already has stone
        if (trainerG.board.get().getStone(event.x, event.y) != 0) {
            trainerG.board.removeMarkup(markupCoord);
            trainerG.board.redraw();
            return;
        }

        trainerG.board.nextButton.disabled = true;
        gameplay.takePlayerControl();

        if (trainerG.board.getNextColor() != trainerG.color) {
            trainerG.setColor();
        }

        trainerG.board.removeMarkup(markupCoord);

        await gameplay.playerTurn(markupCoord);
    }
};

gameplay.playerTurn = async function (markupCoord) {
    let playerTurnId = ++gameplay.playerTurnId;

    await gameplay.handleJumped();
    if (playerTurnId != gameplay.playerTurnId) return;

    if (!gameplay.suggestionsPromise) gameplay.suggestionsPromise = trainerG.analyze();

    await gameplay.suggestionsPromise;
    if (trainerG.isPassed) return;
    if (playerTurnId != gameplay.playerTurnId) return;

    let suggestionToPlay = trainerG.suggestions.get(0);
    trainerG.isRightChoice = false;
    trainerG.isPerfectChoice = false;
    for (let i = 0; i < trainerG.suggestions.length(); i++) {
        if (markupCoord.compare(trainerG.suggestions.get(i).coord)) {
            if (i == 0 || trainerG.suggestions.get(i).visits == trainerG.suggestions.get(0).visits) {
                trainerG.isPerfectChoice = true;
            }

            trainerG.isRightChoice = true;
            suggestionToPlay = trainerG.suggestions.get(i);
            break;
        }
    }

    await gameplay.playerPlay(suggestionToPlay, markupCoord);

    if (gameplay.shouldShowPlayerOptions()) {
        trainerG.board.nextButton.disabled = false;
    } else {
        await gameplay.nextButtonClickListener();
    }
};

gameplay.playerPlay = async function (suggestionToPlay, markupCoord) {
    stats.update();

    if (settings.wrongMoveCorrection || trainerG.isRightChoice) {
        await trainerG.board.play(suggestionToPlay, trainerG.MOVE_TYPE.PLAYER);

        if (!trainerG.isRightChoice) {
            gameplay.chosenNotPlayedCoordHistory.add(markupCoord);
        }
        
        selfplay.enableButton();

        if (!trainerG.isRightChoice) {
            await trainerG.board.draw(markupCoord, "cross");
        }
    } else {
        await trainerG.board.play(await trainerG.analyzeMove(markupCoord), trainerG.MOVE_TYPE.PLAYER);
    }

    if (!cornerPlacer.shouldForce()) {
        gameplay.suggestionsPromise = trainerG.analyze(trainerG.MOVE_TYPE.OPPONENT);
    }
};

gameplay.handleJumped = async function () {
    if (gameplay.isJumped) {
        await kataGo.syncBoard()
        gameplay.suggestionsPromise = trainerG.analyze();
        gameplay.isJumped = false;
    }
};

gameplay.nextButtonClickListener = async function () {
    trainerG.board.nextButton.disabled = true;

    if (trainerG.color == trainerG.board.getColor()) {
        await gameplay.opponentTurn();
    } else {
        trainerG.board.clearMarkups();
        trainerG.board.redraw();
    }
};

gameplay.opponentTurn = async function () {
    let opponentTurnId = ++gameplay.opponentTurnId;

    if (cornerPlacer.shouldForce()) {
        let suggestion = await cornerPlacer.getSuggestion();
        if (opponentTurnId != gameplay.opponentTurnId) return;

        await cornerPlacer.play(suggestion);
    } else {
        await gameplay.suggestionsPromise;
        if (trainerG.isPassed) return;

        let suggestion = trainerG.suggestions.get();
        trainerG.isRightChoice = true;
        trainerG.isPerfectChoice = suggestion.grade == "A";

        if (opponentTurnId != gameplay.opponentTurnId) return;
        await trainerG.board.play(suggestion, trainerG.MOVE_TYPE.OPPONENT);
    }

    gameplay.suggestionsPromise = trainerG.analyze();

    if (gameplay.shouldShowOpponentOptions()) {
        trainerG.board.nextButton.disabled = false;
    }
    
    gameplay.givePlayerControl();
};

gameplay.detectJump = async function (event) {
    if (event.navChange &&
        !event.treeChange &&
        trainerG.board.get().moveNumber != trainerG.board.lastMove.moveNumber + 1
    ) {
        if (!gameplay.isJumped) {
            gameplay.isJumped = true;
            trainerG.isPassed = false;
            trainerG.board.nextButton.disabled = true;
        }

        gameplay.givePlayerControl(false);

        scoreChart.refresh();
        ratioChart.refresh();
    }
};

gameplay.shouldShowPlayerOptions = function () {
    return settings.hideOptions == settings.HIDE_OPTIONS.NEVER ||
        settings.hideOptions == settings.HIDE_OPTIONS.PERFECT && !trainerG.isPerfectChoice ||
        settings.hideOptions == settings.HIDE_OPTIONS.RIGHT && !trainerG.isRightChoice;
};

gameplay.shouldShowOpponentOptions = function () {
    return !cornerPlacer.shouldForce(true) && (
        settings.hideOpponentOptions == settings.HIDE_OPPONENT_OPTIONS.NEVER ||
        settings.hideOpponentOptions == settings.HIDE_OPPONENT_OPTIONS.PERFECT && !trainerG.isPerfectChoice);
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.gameplay) window.trainer.gameplay = gameplay;

export { gameplay };
