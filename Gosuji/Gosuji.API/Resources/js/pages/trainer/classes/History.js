import { trainerG } from "../utils/trainerG";

class History {
    nodeKey;
    nodes = new Set();

    constructor(name, isServerDataReady = trainerG.isLoadingServerData) {
        this.nodeKey = name;

        if (isServerDataReady) {
            this.addMissingNodes();
        }
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

    addMissingNodes(node = trainerG.board.editor.getRoot()) {
        if (node == null) {
            return;
        }

        const data = node[this.nodeKey];
        if (data != null) {
            this.nodes.add(node);
        }

        for (const childNode of node.children) {
            this.addMissingNodes(childNode);
        }
    }

    findFirstInBranch(node=trainerG.board.get()) {
        let data;
        let childNode = node;
        while (childNode) {
            data = childNode[this.nodeKey];
            if (data != null) {
                return data;
            }

            if (childNode.children.length == 0) {
                break;
            }

            childNode = childNode.children[0];
        }

        let parentNode = node.parent;
        while (parentNode) {
            data = parentNode[this.nodeKey];
            if (data != null) {
                return data;
            }

            parentNode = parentNode.parent;
        }
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
