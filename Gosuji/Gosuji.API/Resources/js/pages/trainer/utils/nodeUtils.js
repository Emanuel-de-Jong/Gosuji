import { trainerG } from "./trainerG";

let nodeUtils = { id: "nodeUtils" };


nodeUtils.init = function () { };

nodeUtils.clear = function () { };


nodeUtils.get = function() {
    return trainerG.board.editor.getCurrent();
};


if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.nodeUtils) window.trainer.nodeUtils = nodeUtils;

export { nodeUtils };
