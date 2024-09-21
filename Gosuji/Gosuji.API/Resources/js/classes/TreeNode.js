let TreeNode = window.TreeNode;

if (typeof TreeNode === "undefined") {
    TreeNode = class TreeNode {
        value;
        children = [];

        constructor(value) {
            this.value = value;
        }

        add(childNode) {
            this.children.push(childNode);
        }
    }

    window.TreeNode = TreeNode;
}

export { TreeNode };
