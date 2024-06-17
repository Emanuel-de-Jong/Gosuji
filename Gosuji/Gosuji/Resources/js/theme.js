import { CEvent } from "./classes/CEvent";
import { utils } from "./utils";

let theme = window.theme;

if (typeof theme === "undefined") {
    theme = { id: "theme" };

    theme.TYPES = {
        DARK: 0,
        LIGHT: 1,
    };
    
    theme.init = function () {
        console.log("theme.init");
        theme.themeChangedEvent = new CEvent();

        let tempTheme = utils.getCookie("theme");
        if (tempTheme == null || tempTheme == "") {
            tempTheme = theme.TYPES.DARK;
        } else {
            tempTheme = parseInt(tempTheme);
        }
        
        theme.set(tempTheme);
    };

    theme.nameToNum = function (name) {
        return theme.TYPES[name];
    };

    theme.numToName = function (num) {
        for (const [key, value] of Object.entries(theme.TYPES)) {
            if (value === num) {
                return key;
            }
        }
    };

    theme.set = (newTheme = theme.theme) => {
        document.getElementsByTagName("html")[0].dataset.bsTheme = theme.numToName(newTheme).toLowerCase();

        theme.theme = newTheme;
        theme.themeChangedEvent.dispatch({ theme: theme.theme });

        utils.setCookie("theme", theme.theme);
    };

    window.theme = theme;

    theme.init();
}

export { theme };
