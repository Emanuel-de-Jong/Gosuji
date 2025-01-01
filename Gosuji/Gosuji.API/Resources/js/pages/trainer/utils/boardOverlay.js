let boardOverlay = { id: "boardOverlay" };

boardOverlay.init = function () {
    boardOverlay.element = document.getElementById("boardOverlay");

    boardOverlay.setDynamicHeight();

    boardOverlay.startOverlayElement = document.getElementById("startOverlay");
    boardOverlay.startOverlayStartBtn = document.querySelector("#startOverlay .startBtn");

    boardOverlay.finishedOverlayElement = document.getElementById("finishedOverlay");
    boardOverlay.finishedOverlayCloseBtn = document.querySelector("#finishedOverlay .closeBtn");
    boardOverlay.finishedOverlayNewGameBtn = document.querySelector("#finishedOverlay .newGameBtn");

    boardOverlay.finishedOverlayCloseBtn.addEventListener("click", () => boardOverlay.setVisibilityFinishedOverlay(false));
};

boardOverlay.clear = function () {
    boardOverlay.setVisibilityStartOverlay(false);
    boardOverlay.setVisibilityFinishedOverlay(false);
};

boardOverlay.setDynamicHeight = function () {
    const resizeObserver = new ResizeObserver(entries => {
        for (let entry of entries) {
            const { width } = entry.contentRect;
            boardOverlay.element.style.maxHeight = `${width}px`;
        }
    });

    resizeObserver.observe(boardOverlay.element);
};

boardOverlay.setVisibilityStartOverlay = function (shouldShow) {
    boardOverlay.startOverlay.hidden = !shouldShow;
};

boardOverlay.setVisibilityFinishedOverlay = function (shouldShow) {
    boardOverlay.finishedOverlay.hidden = !shouldShow;
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.boardOverlay) window.trainer.boardOverlay = boardOverlay;

export { boardOverlay };
