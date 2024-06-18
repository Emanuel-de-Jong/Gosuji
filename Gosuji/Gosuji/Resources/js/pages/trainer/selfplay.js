import { settings } from "./utils/settings";
import { sgf } from "./utils/sgf";
import { trainerG } from "./utils/trainerG";
import { gameplay } from "./gameplay";

let selfplay = { id: "selfplay" };


selfplay.init = async function () {
    selfplay.button = document.getElementById("selfplay");
    selfplay.button.addEventListener("click", selfplay.buttonClickListener);

    selfplay.isPlaying = false;
    selfplay.startPromise = null;

    await selfplay.clear();
};

selfplay.clear = async function () {
    if (selfplay.isPlaying) {
        await selfplay.buttonClickListener();
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

        await trainerG.board.play(trainerG.suggestions.get(0), trainerG.MOVE_TYPE.SELFPLAY);
    }
};

selfplay.buttonClickListener = async function () {
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
