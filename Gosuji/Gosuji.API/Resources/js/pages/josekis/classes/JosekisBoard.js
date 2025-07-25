import { josekisPage } from "../josekis"

class JosekisBoard extends Board {
    constructor() {
        super();

        this.besogoOptions.panels = "control";
        this.besogoOptions.tool = "cross";
    }

    async init(boardsize, handicap, sgfContent, stoneVolume) {
        await super.init(boardsize, handicap, sgfContent, stoneVolume);

        this.addMarkup(16, 4, "A");
        this.addMarkup(17, 4, "B");
        this.addMarkup(16, 5, "C");
        this.addMarkup(17, 3, "D");
        this.addMarkup(17, 5, "E");
        this.redrawMarkup();

        document.querySelector('button[title="Jump back"]').remove();
        document.querySelector('button[title="Next node"]').remove();
        document.querySelector('button[title="Jump forward"]').remove();
        document.querySelector('button[title="Last node"]').remove();
        document.querySelector('button[title="Previous sibling"]').remove();
        document.querySelector('button[title="Next sibling"]').remove();
        document.querySelector('button[title="Variants: [child]/sibling"]').remove();
        document.querySelector('button[title="Variants: show/[hide]"]').remove();

        document.querySelector('button[title="First node"]').insertAdjacentHTML("afterend",
            `<button class="bsg lastBranchBtn">
                <svg width="100%" height="100%" viewBox="0 0 100 100">
                    <polygon points="95,10 50,50 50,10 5,50 50,90 50,50 95,90" stroke="none"></polygon>
                </svg>
            </button>`
        );
        document.querySelector('button[title="Previous node"]').insertAdjacentHTML("afterend", '<button style="width: 50px" class="bsg passBtn">Pass</button>');

        document.querySelector(".passBtn").addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("Pass"); });
        document.querySelector(".lastBranchBtn").addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("LastBranch"); });
        document.querySelector('button[title="First node"]').addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("First"); });
        document.querySelector('button[title="Previous node"]').addEventListener("click", async () => { await josekisPage.josekisRef.invokeMethodAsync("Prev"); });

        this.addListener(this.crossPlacedListener);
    }
    
    clearFuture() {
        this.get().children = [];
    }

    crossPlacedListener = async (event) => {
        if (event.markupChange && event.mark == 4) {
            this.removeMarkup(new Coord(event.x, event.y));
            this.redrawBoard();

            await josekisPage.josekisRef.invokeMethodAsync("Next", event.x, event.y);
        }
    }
}

if (!window.josekis) window.josekis = {};
if (g.DEBUG && !window.josekis.JosekisBoard) window.josekis.JosekisBoard = JosekisBoard;

export { JosekisBoard };
