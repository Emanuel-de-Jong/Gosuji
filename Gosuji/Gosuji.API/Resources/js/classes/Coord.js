import { byteUtils } from "../byteUtils";

let Coord = window.Coord;

if (typeof Coord === "undefined") {
    Coord = class Coord {
        x;
        y;


        constructor(x, y) {
            this.x = x;
            this.y = y;
        }


        compare(coord) {
            if (this.x == coord.x && this.y == coord.y) {
                return true;
            }
            return false;
        }


        static fromKataGo(kataGoCoord) {
            return new Coord(kataGoCoord.x, kataGoCoord.y);
        }
    }

    window.Coord = Coord;
}

export { Coord };
