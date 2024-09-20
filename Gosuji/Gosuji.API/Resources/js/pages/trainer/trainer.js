import { History } from "./classes/History";
import { MoveSuggestionList } from "./classes/MoveSuggestionList";
import { MoveSuggestion } from "./classes/MoveSuggestion";
import { Ratio } from "./classes/Ratio";
import { Score } from "./classes/Score";
import { TrainerBoard } from "./classes/TrainerBoard";

import { kataGo } from "./utils/kataGo";
import { scoreChart } from "./utils/scoreChart";
import { ratioChart } from "./utils/ratioChart";
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

let trainerPage = { id: "trainerPage" };


trainerPage.init = async function (
    trainerRef,
    trainerConnectionRef,
    userName,
    stoneVolume,
    isPreMoveStoneSound,
    isSelfplayStoneSound,
    trainerSettingConfig,
    gameLoadInfo
) {
    trainerG.isLoadingServerData = gameLoadInfo != null;

    if (trainerG.isLoadingServerData) {
        debug.testData = 0;
    }

    trainerG.init(trainerRef, gameLoadInfo);
    trainerG.setPhase(trainerG.PHASE_TYPE.INIT);
    await kataGo.init(trainerConnectionRef);
    await settings.init(trainerSettingConfig, gameLoadInfo);
    trainerG.board.init(gameLoadInfo ? gameLoadInfo.boardsize : null,
        gameLoadInfo ? gameLoadInfo.handicap : null,
        gameLoadInfo ? gameLoadInfo.sgf : null,
        stoneVolume,
        isPreMoveStoneSound,
        isSelfplayStoneSound);

    trainerPage.startButton = document.getElementById("startBtn");
    trainerPage.restartButton = document.getElementById("restartBtn");
    trainerPage.restartButton.disabled = true;
    trainerPage.newGameButton = document.getElementById("newGameBtn");
    trainerPage.startButton.addEventListener("click", trainerPage.start);
    trainerPage.restartButton.addEventListener("click", trainerPage.restartButtonClickListener);
    trainerPage.newGameButton.addEventListener("click", trainerPage.restartButtonClickListener);

    sgf.init(userName, gameLoadInfo);
    sgfComment.init();
    scoreChart.init();
    await stats.init(gameLoadInfo);
    ratioChart.init();
    debug.init();
    gameplay.init(gameLoadInfo);
    cornerPlacer.init();
    preMovePlacer.init();
    await selfplay.init();

    sgf.sgfLoadingEvent.add(trainerPage.sgfLoadingListener);
    sgf.sgfLoadedEvent.add(trainerPage.sgfLoadedListener);

    document.getElementById("settingsAccordion").hidden = false;
};

trainerPage.clear = async function () {
    trainerG.setPhase(trainerG.PHASE_TYPE.RESTART);

    trainerPage.restartButton.disabled = true;

    await selfplay.clear();
    trainerG.clear();
    settings.clear();
    trainerG.board.init();
    sgf.clear();
    sgfComment.clear();
    scoreChart.clear();
    await stats.clear();
    ratioChart.clear();
    debug.clear();
    gameplay.clear();
    cornerPlacer.clear();
    preMovePlacer.clear();
    await kataGo.clear();
};


trainerPage.start = async function () {
    trainerG.board.startOverlay.hidden = true;
    trainerPage.restartButton.disabled = false;

    settings.togglePreGameSettings();

    await kataGo.start();
    
    if (trainerG.isLoadingServerData || debug.testData) {
        trainerG.isLoadingServerData = false;
        gameplay.start(false);
    } else {
        preMovePlacer.start();
    }
};

trainerPage.restartButtonClickListener = async function () {
    await trainerPage.clear();
    await trainerPage.start();
};

trainerPage.sgfLoadingListener = async function () {
    await selfplay.clear();
    preMovePlacer.clear();
    trainerG.clear();
};

trainerPage.sgfLoadedListener = async function () {
    sgfComment.clear();
    scoreChart.clear();
    await stats.clear();
    ratioChart.clear();
    gameplay.clear();
    await kataGo.clear();

    gameplay.start(false);
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.trainerPage) window.trainer.trainerPage = trainerPage;

export {
    // History,
    // MoveSuggestion,
    // MoveSuggestionList,
    // Ratio,
    // Score,
    // TrainerBoard,

    // kataGo,
    // scoreChart,
    // ratioChart,
    settings,
    // sgf,
    // sgfComment,
    // stats,
    trainerG,

    // cornerPlacer,
    // debug,
    // gameplay,
    trainerPage,
    // preMovePlacer,
    // selfplay,
};
