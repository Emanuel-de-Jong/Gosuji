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

    if (debug.testData == 1) {
        gameplay.chosenNotPlayedCoordHistory.add(new Coord(8, 12), 2, 0);
        gameplay.chosenNotPlayedCoordHistory.add(new Coord(17, 17), 4, 1);
    }
};


gameplay.givePlayerControl = function (isSuggestionNeeded = true) {
    trainerG.setPhase(trainerG.PHASE_TYPE.GAMEPLAY);

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
        gameplay.takePlayerControl();

        if (trainerG.board.getNextColor() != trainerG.color) {
            trainerG.setColor();
        }

        let markupCoord = new Coord(event.x, event.y);
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
    let opponentOptions = gameplay.getOpponentOptions();

    stats.update();

    if (settings.wrongMoveCorrection || trainerG.isRightChoice) {
        if (!trainerG.isRightChoice) {
            gameplay.chosenNotPlayedCoordHistory.add(markupCoord, trainerG.board.getNodeX() + 1);
        }

        await trainerG.board.play(suggestionToPlay, trainerG.MOVE_TYPE.PLAYER);
        
        selfplay.enableButton();

        if (!trainerG.isRightChoice) {
            await trainerG.board.draw(markupCoord, "cross");
        }
    } else {
        await trainerG.board.play(await trainerG.analyzeMove(markupCoord), trainerG.MOVE_TYPE.PLAYER);
    }

    if (!cornerPlacer.shouldForce()) {
        gameplay.suggestionsPromise = trainerG.analyze(
            settings.opponentVisits,
            opponentOptions,
            gameplay.OPPONENT_MIN_VISITS_PERC,
            gameplay.OPPONENT_MAX_VISIT_DIFF_PERC
        );
    }
};

gameplay.handleJumped = async function () {
    if (gameplay.isJumped) {
        await trainerG.board.syncWithServer();
        gameplay.suggestionsPromise = trainerG.analyze();
        gameplay.isJumped = false;
    }
};

gameplay.getOpponentOptions = function () {
    let opponentOptions = 1;
    if (settings.opponentOptionsSwitch) {
        if (utils.randomInt(100) + 1 <= settings.opponentOptionPerc) {
            opponentOptions = settings.opponentOptions;
        }
    }
    return opponentOptions;
};

gameplay.nextButtonClickListener = async function () {
    trainerG.board.nextButton.disabled = true;
    await gameplay.opponentTurn();
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

        let suggestion;
        if (trainerG.suggestions.length() == 1) {
            suggestion = trainerG.suggestions.get(0);
        } else {
            suggestion = trainerG.suggestions.get(utils.randomInt(trainerG.suggestions.length() - 1) + 1);
        }

        if (opponentTurnId != gameplay.opponentTurnId) return;
        
        await trainerG.board.play(suggestion, trainerG.MOVE_TYPE.OPPONENT);
    }

    gameplay.givePlayerControl();
};

gameplay.detectJump = async function (event) {
    if (event.navChange && !event.treeChange) {
        gameplay.givePlayerControl(false);

        scoreChart.refresh();
        ratioChart.refresh();

        if (!gameplay.isJumped) {
            gameplay.isJumped = true;
            trainerG.isPassed = false;
            trainerG.board.nextButton.disabled = true;
        }
    }
};

gameplay.shouldShowPlayerOptions = function () {
    return settings.hideOptions == settings.HIDE_OPTIONS.NEVER ||
        settings.hideOptions == settings.HIDE_OPTIONS.PERFECT && !trainerG.isPerfectChoice ||
        settings.hideOptions == settings.HIDE_OPTIONS.RIGHT && !trainerG.isRightChoice;
};

export { gameplay };
