var gameplay = {};


gameplay.OPPONENT_MIN_VISITS_PERC = 10;
gameplay.OPPONENT_MAX_VISIT_DIFF_PERC = 50;


gameplay.init = function (serverChosenNotPlayedCoords) {
    gameplay.clear(serverChosenNotPlayedCoords);
};

gameplay.clear = function (serverChosenNotPlayedCoords) {
    gameplay.suggestionsPromise = null;
    gameplay.chosenNotPlayedCoordHistory = serverChosenNotPlayedCoords
        ? History.fromServer(serverChosenNotPlayedCoords, Coord)
        : new History();
    gameplay.isPlayerControlling = false;
    gameplay.isJumped = false;
    gameplay.playerTurnId = 0;
    gameplay.opponentTurnId = 0;

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
    let isRightChoice = false;
    let isPerfectChoice = false;
    for (let i = 0; i < trainerG.suggestions.length(); i++) {
        if (markupCoord.compare(trainerG.suggestions.get(i).coord)) {
            if (i == 0 || trainerG.suggestions.get(i).visits == trainerG.suggestions.get(0).visits) {
                isPerfectChoice = true;
            }

            isRightChoice = true;
            suggestionToPlay = trainerG.suggestions.get(i);
            break;
        }
    }

    await gameplay.playerPlay(isRightChoice, isPerfectChoice, suggestionToPlay, markupCoord);

    if (settings.showOptions) {
        trainerG.board.nextButton.disabled = false;
    } else {
        await gameplay.nextButtonClickListener();
    }
};

gameplay.handleJumped = async function () {
    if (gameplay.isJumped) {
        await trainerG.board.syncWithServer();
        gameplay.suggestionsPromise = trainerG.analyze();
        gameplay.isJumped = false;
    }
};

gameplay.playerPlay = async function (isRightChoice, isPerfectChoice, suggestionToPlay, markupCoord) {
    let opponentOptions = gameplay.getOpponentOptions();

    stats.updateRatioHistory(isRightChoice, isPerfectChoice);

    if (!settings.disableAICorrection || isRightChoice) {
        if (!isRightChoice) {
            gameplay.chosenNotPlayedCoordHistory.add(markupCoord, trainerG.board.getNodeX() + 1);
        }

        await trainerG.board.play(suggestionToPlay, trainerG.MOVE_TYPE.PLAYER);

        if (!isRightChoice) {
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
        if (opponentTurnId != gameplay.opponentTurnId) return;

        await trainerG.board.play(trainerG.suggestions.get(utils.randomInt(trainerG.suggestions.length())), trainerG.MOVE_TYPE.OPPONENT);
    }

    gameplay.givePlayerControl();
};

gameplay.treeJumpedCheckListener = function (event) {
    if (event.navChange) {
        stats.setRatio();
        stats.clearVisits();

        if (
            !event.treeChange ||
            (settings.showOptions && trainerG.color == trainerG.board.getColor()) ||
            (settings.showOpponentOptions && trainerG.color != trainerG.board.getColor())
        ) {
            if (trainerG.phase == trainerG.PHASE_TYPE.GAMEPLAY) {
                if (!event.treeChange || trainerG.board.getNodeX() == 0) trainerG.suggestions = trainerG.suggestionsHistory.get();

                if (trainerG.suggestions) {
                    stats.setVisits(trainerG.suggestions);
                    trainerG.board.drawCoords(trainerG.suggestions);
                }
            }
        }

        if (!event.treeChange) {
            gameplay.givePlayerControl(false);

            scoreChart.refresh();

            if (!gameplay.isJumped) {
                gameplay.isJumped = true;
                trainerG.isPassed = false;
                trainerG.board.nextButton.disabled = true;
            }
        }
    }
};
