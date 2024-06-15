import { Board } from "./classes/Board";
import { CEvent } from "./classes/CEvent";
import { Coord } from "./classes/Coord";
import { byteUtils } from "./byteUtils";
import { g } from "./g";
import { utils } from "./utils";


let custom = window.custom;

if (typeof custom === "undefined") {
    custom = { id: "custom" };

    custom.THEME_TYPES = {
        DARK: 0,
        LIGHT: 1,
    };


    custom.init = function () {
        custom.themeChangedEvent = new CEvent();
        custom.theme = custom.THEME_TYPES.DARK;

        custom.clear();
    };

    custom.clear = function () {
        let copyrightYearElement = document.getElementById("copyrightYear");
        copyrightYearElement.innerHTML = new Date().getFullYear();

        custom.htmlElement = document.getElementsByTagName("html")[0];
    };


    custom.switchTheme = function (toDarkTheme) {
        if ((custom.theme == custom.THEME_TYPES.DARK && toDarkTheme) ||
                (custom.theme == custom.THEME_TYPES.LIGHT && !toDarkTheme)) {
            return;
        }
        
        if (toDarkTheme) {
            custom.enableDarkTheme();
        } else {
            custom.disableDarkTheme();
        }

        custom.themeChangedEvent.dispatch({ theme: custom.theme });
    };

    custom.enableDarkTheme = function () {
        custom.theme = custom.THEME_TYPES.DARK;
        custom.htmlElement.dataset.bsTheme = "dark";
    };

    custom.disableDarkTheme = function () {
        custom.theme = custom.THEME_TYPES.LIGHT;
        custom.htmlElement.dataset.bsTheme = "light";
    };

    custom.fitTextareaToContent = function (textarea) {
        if (textarea.value == "") {
            return;
        }

        textarea.style.height = "auto";
        textarea.style.height = textarea.scrollHeight + 5 + "px";
    };

    window.custom = custom;
}

export { Board, CEvent, Coord, byteUtils, g, utils, custom };
