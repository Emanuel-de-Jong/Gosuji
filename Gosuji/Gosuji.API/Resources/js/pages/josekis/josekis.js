import { JosekisBoard } from "./classes/JosekisBoard"

let josekisPage = {};

josekisPage.init = function (josekisRef, stoneVolume) {
    this.josekisRef = josekisRef;

    this.board = new JosekisBoard();
    this.board.init(19, 0, null, stoneVolume);
};

export { josekisPage };
