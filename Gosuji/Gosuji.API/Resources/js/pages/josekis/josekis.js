import { JosekisBoard } from "./classes/JosekisBoard"

let josekisPage = {};

josekisPage.init = async function (josekisRef, stoneVolume) {
    this.josekisRef = josekisRef;

    this.board = new JosekisBoard();
    await this.board.init(19, 0, null, stoneVolume);
};

if (!window.josekis) window.josekis = {};
if (g.DEBUG && !window.josekis.josekisPage) window.josekis.josekisPage = josekisPage;

export { josekisPage };
