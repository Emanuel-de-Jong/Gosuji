let boardOverlay = { id: "boardOverlay" };

boardOverlay.init = function () {
    boardOverlay.element = document.getElementById("boardOverlay");
    boardOverlay.overlays = [];

    boardOverlay.startOverlay = document.getElementById("startOverlay");
    boardOverlay.overlays.push(boardOverlay.startOverlay);
    boardOverlay.startOverlayStartBtn = document.querySelector("#startOverlay .startBtn");

    boardOverlay.preMoveOverlay = document.getElementById("preMoveOverlay");
    boardOverlay.overlays.push(boardOverlay.preMoveOverlay);
    boardOverlay.preMoveOverlayStopBtn = document.querySelector("#preMoveOverlay .stopBtn");

    boardOverlay.finishedOverlay = document.getElementById("finishedOverlay");
    boardOverlay.overlays.push(boardOverlay.finishedOverlay);
    boardOverlay.finishedOverlayCloseBtn = document.querySelector("#finishedOverlay .closeBtn");
    boardOverlay.finishedOverlayNewGameBtn = document.querySelector("#finishedOverlay .newGameBtn");

    boardOverlay.automateHeight();

    boardOverlay.finishedOverlayCloseBtn.addEventListener("click", () => boardOverlay.finishedOverlay.hidden = true);
};

boardOverlay.clear = function () {
    boardOverlay.hide();
};

boardOverlay.hide = function () {
    for (const overlay of boardOverlay.overlays) {
        overlay.hidden = true;
    }
};

boardOverlay.automateHeight = function () {
    boardOverlay.setDynamicHeight();

    const resizeObserver = new ResizeObserver(entries => {
        for (let entry of entries) {
            if (entry.target == boardOverlay.element) {
                boardOverlay.setDynamicHeight();
            }
        }
    });

    resizeObserver.observe(boardOverlay.element);
}

boardOverlay.setDynamicHeight = function () {
    const width = boardOverlay.element.offsetWidth;
    const newHeight = `${width}px`;

    for (const overlay of boardOverlay.overlays) {
        overlay.style.height = newHeight;
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.boardOverlay) window.trainer.boardOverlay = boardOverlay;

export { boardOverlay };
