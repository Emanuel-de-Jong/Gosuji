var josekisPage = {};

josekisPage.init = function () {
    josekisPage.boardElement = document.getElementById("board");

    let besogoOptions = {
        resize: "auto",
        orient: "portrait",
        panels: "control+names+tree+comment+file",
        coord: "western",
        tool: "navOnly",
        size: 19,
        variants: 2,
        nowheel: true,
    };

    besogo.create(josekisPage.boardElement, besogoOptions);

    josekisPage.boardEditor = josekisPage.boardElement.besogoEditor;
};