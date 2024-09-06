class Move {
    color;
    coord;

    constructor(color, coord) {
        this.color = color;
        this.coord = coord;
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.Move) window.trainer.Move = Move;

export { Move };
