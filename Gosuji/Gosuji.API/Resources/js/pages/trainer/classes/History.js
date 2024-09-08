import { trainerG } from "../utils/trainerG";

class History {
    nodeKey;
    nodes = new Set();

    constructor() {
        this.nodeKey = utils.randomString();
    }

    add(data, node=trainerG.board.get()) {
        node[this.nodeKey] = data;
        this.nodes.add(node);
    }

    addWithCoord(data, x=trainerG.board.getNodeX(), y=trainerG.board.getNodeY()) {
        const node = trainerG.board.findNode(new Coord(x, y));
        return this.add(data, node);
    }

    get(node=trainerG.board.get()) {
        if (node == null) {
            return null;
        }

        return node[this.nodeKey];
    }

    getWithCoord(x=trainerG.board.getNodeX(), y=trainerG.board.getNodeY()) {
        for (const node of this.nodes) {
            if (node.navTreeX == x && node.navTreeY == y) {
                return this.get(node);
            }
        }
    }

    has(node=trainerG.board.get()) {
        return this.get(node) != null;
    }

    refreshNodes() {
        for (const node of this.nodes) {
            if (!node) {
                this.nodes.delete(node);
            }
        }
    }

    getHighestX() {
        let highestXNode;
        for (const node of this.nodes) {
            if (!highestXNode || node.navTreeX > highestXNode.navTreeX) {
                highestXNode = node;
            }
        }
        return highestXNode;
    }

    count(data) {
        let count = 0;
        for (const node of this.nodes) {
            if (this.get(node) == data) {
                count++;
            }
        }
        return count;
    }

    *iterate() {
        for (const node of this.nodes) {
            yield this.get(node);
        }
    }

    static fromServer() {
        return new History();
    }

    // encode() {
    //     if (!this.dataType) return [];

    //     let encoded = [];

    //     for (const gridPoint of this.iterateGrid()) {
    //         if (!gridPoint.data) {
    //             encoded = byteUtils.numToBytes(this.ENCODE_Y_INDICATOR, 2, encoded);
    //             encoded = byteUtils.numToBytes(gridPoint.y, 2, encoded);
    //         } else {
    //             encoded = byteUtils.numToBytes(gridPoint.x, 2, encoded);

    //             if (this.dataType == "object") {
    //                 encoded = encoded.concat(gridPoint.data.encode());
    //             } else {
    //                 encoded = byteUtils.numToBytes(gridPoint.data, 1, encoded);
    //             }
    //         }
    //     }

    //     return encoded;
    // }

    // *iterateGrid() {
    //     for (let y = 0; y < this.grid.length; y++) {
    //         if (!this.grid[y]) continue;
    //         yield {
    //             y: y,
    //             x: null,
    //             data: null,
    //         };

    //         for (let x = 0; x < this.grid[y].length; x++) {
    //             if (!this.grid[y][x]) continue;
    //             yield {
    //                 y: y,
    //                 x: x,
    //                 data: this.grid[y][x],
    //             };
    //         }
    //     }
    // }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.History) window.trainer.History = History;

export { History };
