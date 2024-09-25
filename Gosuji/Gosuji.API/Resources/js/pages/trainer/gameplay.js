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

gameplay.CHOSEN_NOT_PLAYED_COORD_HISTORY_NAME = "chosenNotPlayedCoord";

gameplay.OPPONENT_MIN_VISITS_PERC = 10;
gameplay.OPPONENT_MAX_VISIT_DIFF_PERC = 50;


gameplay.init = function () {
    trainerG.board.addListener(gameplay.playerMarkupPlacedCheckListener);
    trainerG.board.addListener(gameplay.detectJump);
    trainerG.board.nextButton.addEventListener("click", gameplay.nextButtonClickListener);

    gameplay.clear();
};

gameplay.clear = function () {
    gameplay.suggestionsPromise = null;
    gameplay.chosenNotPlayedCoordHistory = new History(gameplay.CHOSEN_NOT_PLAYED_COORD_HISTORY_NAME);
    gameplay.isPlayerControlling = false;
    gameplay.isJumped = false;
    gameplay.playerTurnId = 0;
    gameplay.opponentTurnId = 0;
};


gameplay.start = function (isSuggestionNeeded = true) {
    trainerG.setPhase(trainerG.PHASE_TYPE.GAMEPLAY);

    if (trainerG.color == trainerG.board.getColor()) {
        if (!cornerPlacer.shouldForce()) {
            gameplay.suggestionsPromise = kataGo.analyze(trainerG.MOVE_ORIGIN.OPPONENT);
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
        gameplay.suggestionsPromise = kataGo.analyze();
    }
};

gameplay.takePlayerControl = function () {
    trainerG.board.editor.setTool("navOnly");
    gameplay.isPlayerControlling = false;
};

gameplay.playerMarkupPlacedCheckListener = async function (event) {
    if (event.markupChange && event.mark == 4 && gameplay.isPlayerControlling && !sgf.isSGFLoading) {
        gameplay.playerPlayedCoord = new Coord(event.x, event.y);

        // Coord already has stone
        if (trainerG.board.get().getStone(event.x, event.y) != 0) {
            trainerG.board.removeMarkup(gameplay.playerPlayedCoord);
            trainerG.board.redrawMarkup();
            return;
        }

        trainerG.board.nextButton.disabled = true;
        gameplay.takePlayerControl();

        if (trainerG.board.getNextColor() != trainerG.color) {
            trainerG.setColor();
        }

        await gameplay.playerTurn();
    }
};

gameplay.playerTurn = async function () {
    let playerTurnId = ++gameplay.playerTurnId;

    if (gameplay.isJumped) {
        gameplay.isJumped = false;
        gameplay.suggestionsPromise = kataGo.analyze(
            trainerG.MOVE_ORIGIN.PLAYER,
            trainerG.board.getNextColor(),
            true);
        await gameplay.suggestionsPromise;
    }
    if (playerTurnId != gameplay.playerTurnId) return;

    if (!gameplay.suggestionsPromise) gameplay.suggestionsPromise = kataGo.analyze();

    await gameplay.suggestionsPromise;
    if (trainerG.isPassed) return;
    if (playerTurnId != gameplay.playerTurnId) return;

    let suggestionToPlay = trainerG.suggestions.get(0);
    trainerG.isRightChoice = false;
    trainerG.isPerfectChoice = false;
    for (let i = 0; i < trainerG.suggestions.length(); i++) {
        if (gameplay.playerPlayedCoord.compare(trainerG.suggestions.get(i).coord)) {
            if (trainerG.suggestions.get(i).grade == "A") {
                trainerG.isPerfectChoice = true;
            }

            trainerG.isRightChoice = true;
            suggestionToPlay = trainerG.suggestions.get(i);
            break;
        }
    }

    stats.update();

    trainerG.board.removeMarkup(gameplay.playerPlayedCoord);

    if (settings.wrongMoveCorrection || trainerG.isRightChoice) {
        await trainerG.board.play(suggestionToPlay, trainerG.MOVE_ORIGIN.PLAYER);
    } else {
        await trainerG.board.play(await kataGo.analyzeMove(gameplay.playerPlayedCoord), trainerG.MOVE_ORIGIN.PLAYER);
    }

    selfplay.enableButton();

    if (!cornerPlacer.shouldForce()) {
        gameplay.suggestionsPromise = kataGo.analyze(trainerG.MOVE_ORIGIN.OPPONENT);
    }

    if (gameplay.shouldShowPlayerOptions()) {
        trainerG.board.nextButton.disabled = false;
    } else {
        await gameplay.nextButtonClickListener();
    }
};

gameplay.nextButtonClickListener = async function () {
    trainerG.board.nextButton.disabled = true;

    if (trainerG.color == trainerG.board.getColor()) {
        await gameplay.opponentTurn();
    } else {
        trainerG.board.clearMarkups(true);
        trainerG.board.redrawMarkup();
    }
};

gameplay.opponentTurn = async function () {
    let opponentTurnId = ++gameplay.opponentTurnId;

    if (cornerPlacer.shouldForce()) {
        await cornerPlacer.play();
    } else {
        await gameplay.suggestionsPromise;
        if (trainerG.isPassed) return;

        let suggestion = trainerG.suggestions.get();
        trainerG.isRightChoice = true;
        trainerG.isPerfectChoice = suggestion.grade == "A";

        if (opponentTurnId != gameplay.opponentTurnId) return;
        await trainerG.board.play(suggestion, trainerG.MOVE_ORIGIN.OPPONENT);
    }

    gameplay.suggestionsPromise = kataGo.analyze();

    if (gameplay.shouldShowOpponentOptions()) {
        trainerG.board.nextButton.disabled = false;
    }
    
    gameplay.givePlayerControl();
};

gameplay.detectJump = function (event) {
    if (event.navChange &&
        !event.treeChange &&
        trainerG.board.get().moveNumber != trainerG.board.lastNode.moveNumber + 1
    ) {
        if (!gameplay.isJumped) {
            gameplay.isJumped = true;
            trainerG.isPassed = false;
            trainerG.board.nextButton.disabled = true;
        }

        gameplay.givePlayerControl(false);

        stats.setResult();
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
