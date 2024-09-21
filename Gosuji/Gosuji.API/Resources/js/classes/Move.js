let Move = window.Move;

if (typeof Move === "undefined") {
    Move = class Move {
        color;
        coord;
    
        constructor(color, coord) {
            this.color = color;
            this.coord = coord;
        }
    }

    window.Move = Move;
}

export { Move };
