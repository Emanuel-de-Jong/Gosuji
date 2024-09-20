import { kataGo } from "./utils/kataGo";
import { settings } from "./utils/settings";
import { sgf } from "./utils/sgf";
import { trainerG } from "./utils/trainerG";
import { gameplay } from "./gameplay";

let selfplay = { id: "selfplay" };


selfplay.PLAYER_MOVES_BEFORE_ENABLE = 10;


selfplay.init = async function () {
    selfplay.button = document.getElementById("selfplayBtn");
    selfplay.button.addEventListener("click", selfplay.toggleSelfplay);

    selfplay.isPlaying = false;
    selfplay.startPromise = null;
    selfplay.lastMoveTime = 0;

    await selfplay.clear();
};

selfplay.clear = async function () {
    selfplay.button.disabled = true;

    if (selfplay.isPlaying) {
        await selfplay.toggleSelfplay();
    }
};


selfplay.enableButton = function () {
    if (selfplay.button.disabled == true &&
        trainerG.moveTypeHistory.count(trainerG.MOVE_TYPE.PLAYER) >= selfplay.PLAYER_MOVES_BEFORE_ENABLE
    ) {
        selfplay.button.disabled = false;
    }
};

selfplay.start = async function () {
    trainerG.setPhase(trainerG.PHASE_TYPE.SELFPLAY);

    await gameplay.handleJumped();

    while (selfplay.isPlaying || trainerG.color != trainerG.board.getNextColor()) {
        await kataGo.analyze(trainerG.MOVE_TYPE.SELFPLAY);
        if (trainerG.isPassed) {
            selfplay.button.click();
            return;
        }

        if (selfplay.isPlaying) {
            const timeToWait = selfplay.lastMoveTime + settings.selfplayPlaySpeed * 1000 - Date.now();
            if (timeToWait > 0) {
                await Promise.race([
                    new Promise(resolve => setTimeout(resolve, timeToWait)),
                    new Promise(resolve => {
                        const interval = setInterval(() => {
                            if (!selfplay.isPlaying) {
                                clearInterval(interval);
                                resolve();
                            }
                        }, 100);
                    })
                ]);
            }
        }

        if (!selfplay.isPlaying && trainerG.color == trainerG.board.getNextColor()) return;

        selfplay.lastMoveTime = Date.now();
        
        await trainerG.board.play(trainerG.suggestions.get(0), trainerG.MOVE_TYPE.SELFPLAY);
    }
};

selfplay.toggleSelfplay = async function () {
    if (!selfplay.isPlaying) {
        selfplay.isPlaying = true;
        selfplay.button.textContent = "Stop selfplay";

        gameplay.takePlayerControl();
        trainerG.board.nextButton.disabled = true;

        selfplay.startPromise = selfplay.start();
    } else {
        selfplay.isPlaying = false;
        selfplay.button.textContent = "Start selfplay";

        await selfplay.startPromise;

        if (!trainerG.isPassed && !sgf.isSGFLoading) {
            gameplay.start();
        }
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.selfplay) window.trainer.selfplay = selfplay;

export { selfplay };
