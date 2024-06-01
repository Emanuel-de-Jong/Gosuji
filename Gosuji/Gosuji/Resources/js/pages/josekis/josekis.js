import { JosekisBoard } from "./classes/JosekisBoard.js"

let josekisPage = {};

josekisPage.init = function (josekisRef) {
    this.josekisRef = josekisRef;

    this.board = new JosekisBoard();
    this.board.init(19, 0);

    document.querySelector('button[title="Previous node"]').addEventListener("click", async () => {
        await this.josekisRef.invokeMethodAsync("Prev");
    });
};

export { josekisPage };
