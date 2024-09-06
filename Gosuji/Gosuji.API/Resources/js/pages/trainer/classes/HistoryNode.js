import { trainerG } from "../utils/trainerG";

class HistoryNode {
    parent;
    children = [];

    data;
    boardNode;

    constructor(parent, data, boardNode) {
        this.parent = parent;
        this.data = data;
        this.boardNode = boardNode;
    }

    add(data, boardNode) {
        this.children.push(new HistoryNode(this, data, boardNode));
    }

    x() {
        return this.boardNode.navTreeX;
    }

    y() {
        return this.boardNode.navTreeY;
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.HistoryNode) window.trainer.HistoryNode = HistoryNode;

export { HistoryNode };
