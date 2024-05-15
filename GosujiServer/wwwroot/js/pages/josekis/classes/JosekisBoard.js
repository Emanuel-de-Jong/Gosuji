class JosekisBoard extends Board {
    constructor() {
        super();

        this.besogoOptions.panels = "control+comment";
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
    }
}
