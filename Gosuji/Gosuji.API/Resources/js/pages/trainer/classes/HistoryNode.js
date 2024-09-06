import { trainerG } from "../utils/trainerG";

export class HistoryNode {
    data;
    parent;
    children = [];
    x;

    constructor(parent, data, x) {
        this.parent = parent;
        this.data = data;
        this.x = x;
    }

    add(data, x) {
        if (x == null) {
            x = this.x + 1;
        }

        this.children.push(new HistoryNode(this, data, x));
    }
}
