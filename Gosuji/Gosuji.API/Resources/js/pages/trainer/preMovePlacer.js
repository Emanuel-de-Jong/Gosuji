import { boardOverlay } from "./utils/boardOverlay";
import { kataGo } from "./utils/kataGo";
import { settings } from "./utils/settings";
import { sgf } from "./utils/sgf";
import { trainerG } from "./utils/trainerG";
import { cornerPlacer } from "./cornerPlacer";
import { gameplay } from "./gameplay";

let preMovePlacer = { id: "preMovePlacer" };


preMovePlacer.MIN_VISITS_PERC = 10;
preMovePlacer.MAX_VISIT_DIFF_PERC = 50;


preMovePlacer.init = function () {
    boardOverlay.preMoveOverlayStopBtn.addEventListener("click", preMovePlacer.stopButtonClickListener);
    
    preMovePlacer.clear();
    preMovePlacer.isInitialized = true;
};

preMovePlacer.clear = function () {
    preMovePlacer.isStopped = true;
};


preMovePlacer.start = async function () {
    trainerG.setPhase(trainerG.PHASE_TYPE.PREMOVES);

    preMovePlacer.isStopped = false;

    if (settings.preMovesSwitch) {
        boardOverlay.preMoveOverlay.hidden = false;

        for (let i = 0; i < settings.preMoves; i++) {
            if (preMovePlacer.isStopped) break;

            if (cornerPlacer.shouldForce()) {
                await cornerPlacer.play();
            } else {
                await preMovePlacer.play();
            }
        }
    }

    while (trainerG.color != trainerG.board.getNextColor() && !trainerG.isPassed && !sgf.isSGFLoading) {
        await preMovePlacer.play(true);
    }

    boardOverlay.preMoveOverlay.hidden = true;

    if (!trainerG.isPassed && !sgf.isSGFLoading) {
        gameplay.start();
    }
};

preMovePlacer.stopButtonClickListener = function () {
    preMovePlacer.isStopped = true;
};

preMovePlacer.play = async function (isForced = false) {
    if (!isForced && preMovePlacer.isStopped) return;

    await kataGo.analyze(trainerG.MOVE_ORIGIN.PRE);
    if (trainerG.isPassed) preMovePlacer.isStopped = true;
    if (!isForced && preMovePlacer.isStopped) return;

    let suggestion = trainerG.suggestions.get();
    await trainerG.board.play(suggestion, trainerG.MOVE_ORIGIN.PRE);
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.preMovePlacer) window.trainer.preMovePlacer = preMovePlacer;

export { preMovePlacer };
