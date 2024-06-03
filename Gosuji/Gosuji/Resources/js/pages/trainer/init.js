import { CNode } from "./classes/CNode";
import { History } from "./classes/History";
import { MoveSuggestionList } from "./classes/MoveSuggestionList";
import { MoveSuggestion } from "./classes/MoveSuggestion";
import { Ratio } from "./classes/Ratio";
import { Score } from "./classes/Score";
import { TrainerBoard } from "./classes/TrainerBoard";

import { db } from "./utils/db";
import { katago } from "./utils/katago";
import { scoreChart } from "./utils/scoreChart";
import { settings } from "./utils/settings";
import { sgfComment } from "./utils/sgfComment";
import { sgf } from "./utils/sgf";
import { stats } from "./utils/stats";
import { trainerG } from "./utils/trainerG";

import { cornerPlacer } from "./cornerPlacer";
import { debug } from "./debug";
import { gameplay } from "./gameplay";
import { preMovePlacer } from "./preMovePlacer";
import { selfplay } from "./selfplay";

let init = {};


init.init = async function (
    trainerRef,
    kataGoServiceRef,
    userId,
    userName,
    kataGoVersion,

    serverBoardsize,
    serverHandicap,
    serverColor,
    serverKomi,
    serverRuleset,
    serverSGF,
    serverRatios,
    serverSuggestions,
    serverMoveTypes,
    serverChosenNotPlayedCoords
) {
    trainerG.isLoadingServerData = serverSGF != null;

    if (trainerG.isLoadingServerData) {
        debug.testData = 0;
    }

    // console.log(serverBoardsize);
    // console.log(serverHandicap);
    // console.log(serverColor);
    // console.log(serverKomi);
    // console.log(serverRuleset);
    // console.log(serverSGF);
    // console.log(serverRatios);
    // console.log(serverSuggestions);
    // console.log(serverMoveTypes);
    // console.log(serverChosenNotPlayedCoords);

    trainerG.init(trainerRef, kataGoServiceRef, kataGoVersion, serverSuggestions, serverMoveTypes);
    trainerG.setPhase(trainerG.PHASE_TYPE.INIT);

    init.restartButton = document.getElementById("restart");
    init.restartButton.addEventListener("click", init.restartButtonClickListener);

    settings.init(serverColor);
    trainerG.board.init(serverBoardsize, serverHandicap, serverSGF);
    await sgf.init(userName, serverKomi, serverRuleset);
    sgfComment.init();
    scoreChart.init();
    stats.init(serverRatios);
    debug.init();
    gameplay.init(serverChosenNotPlayedCoords);
    cornerPlacer.init();
    preMovePlacer.init();
    await selfplay.init();
    await katago.init(userId);
    db.init();

    // console.log(stats.ratioHistory);
    // console.log(trainerG.suggestionsHistory);
    // console.log(trainerG.moveTypeHistory);
    // console.log(gameplay.chosenNotPlayedCoordHistory);
    // console.log(scoreChart.history);

    sgf.sgfLoadingEvent.add(init.sgfLoadingListener);
    sgf.sgfLoadedEvent.add(init.sgfLoadedListener);

    await init.start();
};

init.clear = async function () {
    trainerG.setPhase(trainerG.PHASE_TYPE.INIT);

    trainerG.clear();
    settings.clear();
    trainerG.board.clear();
    await sgf.clear();
    sgfComment.clear();
    scoreChart.clear();
    stats.clear();
    debug.clear();
    gameplay.clear();
    cornerPlacer.clear();
    preMovePlacer.clear();
    await selfplay.clear();
    await katago.clear();
    db.clear();

    await init.start();
};


init.start = async function () {
    if (trainerG.isLoadingServerData || debug.testData) {
        trainerG.isLoadingServerData = false;
        gameplay.givePlayerControl(false);
    } else {
        await preMovePlacer.start();
    }
};

init.restartButtonClickListener = async function () {
    await init.clear();
};

init.sgfLoadingListener = async function () {
    preMovePlacer.clear();
    await selfplay.clear();
    trainerG.clear();
};

init.sgfLoadedListener = async function () {
    sgfComment.clear();
    scoreChart.clear();
    stats.clear();
    gameplay.clear();

    await katago.clearBoard();
    await katago.setBoardsize();
    await katago.setHandicap();

    gameplay.givePlayerControl();
};

export {
    CNode,
    History,
    MoveSuggestion,
    MoveSuggestionList,
    Ratio,
    Score,
    TrainerBoard,

    db,
    katago,
    scoreChart,
    settings,
    sgf,
    sgfComment,
    stats,
    trainerG,

    cornerPlacer,
    debug,
    gameplay,
    init,
    preMovePlacer,
    selfplay,
};
