import { Board } from "./classes/Board";
import { CEvent } from "./classes/CEvent";
import { Coord } from "./classes/Coord";
import { Move } from "./classes/Move";
import { TreeNode } from "./classes/TreeNode";
import { byteUtils } from "./byteUtils";
import { g } from "./g";
import { theme } from "./theme";
import { utils } from "./utils";


let custom = window.custom;

if (typeof custom === "undefined") {
    custom = { id: "custom" };

    custom.init = function () {
        custom.clear();
    };

    custom.clear = function () {
        let copyrightYearElement = document.getElementById("copyrightYear");
        if (copyrightYearElement != null) {
            copyrightYearElement.textContent = new Date().getFullYear();
        }
    };

    custom.fitTextareaToContent = function (textarea) {
        if (textarea.value == "") {
            return;
        }

        textarea.style.height = "auto";
        textarea.style.height = textarea.scrollHeight + 5 + "px";
    };

    window.custom = custom;

    window.onload = () => custom.init();
}

export { Board, CEvent, Coord, Move, TreeNode,
    byteUtils, g, theme, utils, custom };
