import { josekisPage } from "../josekis.js"

export class JosekisBoard extends Board {
    constructor() {
        super();

        this.besogoOptions.panels = "control";
        this.besogoOptions.tool = "cross";
    }

    init(boardsize, handicap, sgf) {
        super.init(boardsize, handicap, sgf);
    }

    clear(boardsize, handicap, sgf) {
        super.clear(boardsize, handicap, sgf);

        document.querySelector('button[title="Jump back"]').remove();
        document.querySelector('button[title="Next node"]').remove();
        document.querySelector('button[title="Jump forward"]').remove();
        document.querySelector('button[title="Last node"]').remove();
        document.querySelector('button[title="Previous sibling"]').remove();
        document.querySelector('button[title="Next sibling"]').remove();
        document.querySelector('button[title="Variants: [child]/sibling"]').remove();
        document.querySelector('button[title="Variants: show/[hide]"]').remove();

        document.querySelector('button[title="First node"]').insertAdjacentHTML("afterend",
            `<button class="lastBranchBtn">
                <svg width="100%" height="100%" viewBox="0 0 100 100">
                    <polygon points="95,10 50,50 50,10 5,50 50,90 50,50 95,90" stroke="none"></polygon>
                </svg>
            </button>`
        );
        document.querySelector('button[title="Previous node"]').insertAdjacentHTML("afterend", '<button style="width: 50px" class="passBtn">Pass</button>');

        document.querySelector(".passBtn").addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("Pass"); });
        document.querySelector(".lastBranchBtn").addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("LastBranch"); });
        document.querySelector('button[title="First node"]').addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("First"); });
        this.editor.addListener(this.crossPlacedListener);
    }

    clearFuture() {
        this.editor.getCurrent().children = [];
    }

    addMarkup(x, y, markup) {
        this.editor.getCurrent().markup[(x - 1) * 19 + (y - 1)] = markup;
    }

    redraw() {
        this.editor.notifyListeners({ markupChange: true });
    }

    crossPlacedListener = async (event) => {
        if (event.markupChange && event.mark == 4) {
            this.removeMarkup(new Coord(event.x, event.y));
            this.editor.notifyListeners({ stoneChange: true });

            await josekisPage.josekisRef.invokeMethodAsync("Next", event.x - 1, event.y - 1);
        }
    }
}
