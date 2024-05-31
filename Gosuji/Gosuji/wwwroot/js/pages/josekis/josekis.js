if (typeof josekisPage === "undefined") {
    var josekisPage = {};

    josekisPage.init = function (josekisRef) {
        josekisPage.josekisRef = josekisRef;

        josekisPage.board = new JosekisBoard();
        josekisPage.board.init(19, 0);

        document.querySelector('button[title="Previous node"]').addEventListener("click", async () => { await josekisRef.invokeMethodAsync("Prev") });
    };
}

export { josekisPage };
