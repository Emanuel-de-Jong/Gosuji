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
        await trainerG.analyze(settings.selfplayVisits, 1);
        if (trainerG.isPassed) {
            selfplay.button.click();
            return;
        }

        if (!selfplay.isPlaying && trainerG.color == trainerG.board.getNextColor()) return;

        const timeToWait = selfplay.lastMoveTime + settings.selfplayPlaySpeed * 1000 - Date.now();
        if (timeToWait > 0) {
            await utils.sleep(timeToWait);
        }

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
            gameplay.givePlayerControl();
        }
    }
};

export { selfplay };
