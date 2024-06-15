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
    
        let tempTheme = utils.getCookie("theme");
        if (tempTheme == null || tempTheme == "") {
            tempTheme = theme.TYPES.DARK;
            utils.setCookie("theme", tempTheme);
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

    theme.set = (newTheme) => {
        if (theme.theme === newTheme) {
            return;
        }
        
        document.getElementsByTagName("html")[0].dataset.bsTheme = theme.numToName(newTheme).toLowerCase();

        theme.theme = newTheme;
        theme.themeChangedEvent.dispatch({ theme: theme.theme });

        utils.setCookie("theme", theme.theme);
    };

    theme.init();

    window.theme = theme;
}

export { theme };
