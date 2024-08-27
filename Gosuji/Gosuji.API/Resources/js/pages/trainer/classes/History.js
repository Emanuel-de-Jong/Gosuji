import { trainerG } from "../utils/trainerG";

export class History {
    static ENCODE_Y_INDICATOR = -1;

    grid;
    dataType;


    constructor() {
        this.grid = [];
    }


    add(data, x = trainerG.board.getNodeX(), y = trainerG.board.getNodeY()) {
        if (!this.dataType) this.dataType = typeof data;

        if (!this.grid[y]) this.grid[y] = [];
        this.grid[y][x] = data;
    }

    get(x = trainerG.board.getNodeX(), y = trainerG.board.getNodeY()) {
        if (!this.grid[y]) return;
        return this.grid[y][x];
    }

    delete(node = trainerG.board.editor.getCurrent()) {
        this.deleteLoop(node);
        this.fixWrongLengths();
    }

    deleteLoop(node) {
        let x = node.navTreeX;
        let y = node.navTreeY;

        for (let i = 0; i < node.children.length; i++) {
            this.delete(node.children[i]);
        }

        if (!this.grid[y]) return;
        delete this.grid[y][x];

        let index = this.grid[y].length - 1;
        while (index >= 0 && this.grid[y][index] === undefined) {
            index--;
        }
        this.grid[y].length = index + 1;

        if (this.grid[y].length == 0) {
            delete this.grid[y];
        }
    }

    fixWrongLengths() {
        for (let y = 0; y < this.grid.length; y++) {
            if (!this.grid[y]) continue;

            let index = this.getLengthIndex(this.grid[y]);
            this.grid[y].length = index + 1;

            if (this.grid[y].length == 0) {
                delete this.grid[y];
            }
        }

        let index = this.getLengthIndex(this.grid);
        this.grid.length = index + 1;
    }

    getLengthIndex(arr) {
        let index = -1;
        for (let i = arr.length - 1; i >= 0; i--) {
            if (arr[i] !== undefined) {
                index = i;
                break;
            }
        }
        return index;
    }

    hasY(y) {
        return this.grid[y] != null;
    }

    count(data) {
        let count = 0;
        const y = trainerG.board.getNodeY();
        for (let x = 0; x < this.grid[y].length; x++) {
            if (this.grid[y][x] == data) {
                count++;
            }
        }
        return count;
    }

    encode() {
        if (!this.dataType) return [];

        let encoded = [];

        for (const gridPoint of this.iterateGrid()) {
            if (!gridPoint.data) {
                encoded = byteUtils.numToBytes(this.ENCODE_Y_INDICATOR, 2, encoded);
                encoded = byteUtils.numToBytes(gridPoint.y, 2, encoded);
            } else {
                encoded = byteUtils.numToBytes(gridPoint.x, 2, encoded);

                if (this.dataType == "object") {
                    encoded = encoded.concat(gridPoint.data.encode());
                } else {
                    encoded = byteUtils.numToBytes(gridPoint.data, 1, encoded);
                }
            }
        }

        return encoded;
    }


    *iterateData() {
        for (let y = 0; y < this.grid.length; y++) {
            if (!this.grid[y]) continue;
            for (let x = 0; x < this.grid[y].length; x++) {
                if (!this.grid[y][x]) continue;
                yield this.grid[y][x];
            }
        }
    }

    *iterateGrid() {
        for (let y = 0; y < this.grid.length; y++) {
            if (!this.grid[y]) continue;
            yield {
                y: y,
                x: null,
                data: null,
            };

            for (let x = 0; x < this.grid[y].length; x++) {
                if (!this.grid[y][x]) continue;
                yield {
                    y: y,
                    x: x,
                    data: this.grid[y][x],
                };
            }
        }
    }


    static fromServer(serverHistory, dataClass) {
        let history = new History();
        for (let y in serverHistory) {
            for (let x in serverHistory[y]) {
                let data = serverHistory[y][x];
                if (dataClass) {
                    data = dataClass.fromServer(data);
                }

                history.add(data, x, y);
            }
        }

        return history;
    }
}
