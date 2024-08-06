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
        theme.themeChangedEvent = new CEvent();

        let tempTheme = utils.getLocal("theme");
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
        let htmlElement = document.getElementsByTagName("html")[0];
        let newThemeName = theme.numToName(newTheme).toLowerCase();
        if (newThemeName == htmlElement.dataset.bsTheme) return;

        htmlElement.dataset.bsTheme = newThemeName;

        theme.theme = newTheme;
        theme.themeChangedEvent.dispatch({ theme: theme.theme });

        utils.setLocal("theme", theme.theme);
    };

    window.theme = theme;

    theme.init();
}

export { theme };
