import { trainerG } from "../utils/trainerG";

class History {
    nodeKey;
    nodes = new Set();

    constructor() {
        this.nodeKey = utils.randomString();
    }

    add(data, node=trainerG.board.get()) {
        if (data == null) {
            return;
        }
        
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
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.History) window.trainer.History = History;

export { History };
