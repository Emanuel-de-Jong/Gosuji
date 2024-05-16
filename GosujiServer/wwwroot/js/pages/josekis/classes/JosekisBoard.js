class JosekisBoard extends Board {
    constructor() {
        super();

        this.besogoOptions.panels = "control+comment";
        this.besogoOptions.tool = "cross";
    }

    init(boardsize, handicap, sgf) {
        super.init(boardsize, handicap, sgf);
    }

    clear(boardsize, handicap, sgf) {
        super.clear(boardsize, handicap, sgf);
    
        // document.querySelector('#game button[title="Variants: [child]/sibling"]').remove();
        // document.querySelector('#game button[title="Variants: show/[hide]"]').remove();
        // document.querySelector('#game input[value="9x9"]').remove();
        // document.querySelector('#game input[value="13x13"]').remove();
        // document.querySelector('#game input[value="19x19"]').remove();
        // document.querySelector('#game input[value="?x?"]').remove();
        // document.querySelector('#game input[value="Comment"]').remove();
        // document.querySelector('#game input[value="Edit Info"]').remove();
        // document.querySelector('#game input[value="Info"]').remove();

        this.editor.addListener(this.crossPlacedListener);
    }

    addMarkup(x, y, markup) {
        this.editor.getCurrent().markup[(x - 1)*19 + (y - 1)] = markup;
    }

    redraw() {
        this.editor.notifyListeners({ markupChange: true });
    }

    crossPlacedListener = async (event) => {
        if (event.markupChange && event.mark == 4) {
            this.removeMarkup(new Coord(event.x, event.y));
            this.editor.notifyListeners({ stoneChange: true });

            await josekisPage.josekisRef.invokeMethodAsync("CrossPlacedListener", event.x - 1, event.y - 1);
        }
    }
}
