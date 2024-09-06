import { JosekisBoard } from "./classes/JosekisBoard"

let josekisPage = {};

josekisPage.init = function (josekisRef, stoneVolume) {
    this.josekisRef = josekisRef;

    this.board = new JosekisBoard();
    this.board.init(19, 0, null, stoneVolume);
};

if (!window.josekis) window.josekis = {};
if (g.DEBUG && !window.josekis.josekisPage) window.josekis.josekisPage = josekisPage;

export { josekisPage };
