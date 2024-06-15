import { CEvent } from "./classes/CEvent";

let theme = window.theme;

if (typeof theme === "undefined") {
    theme = { id: "theme" };

    theme.TYPES = {
        DARK: 0,
        LIGHT: 1,
    };
    
    
    theme.theme = theme.TYPES.DARK;
    theme.themeChangedEvent = new CEvent();


    theme.switch = function (toDarkTheme) {
        if ((theme.theme == theme.TYPES.DARK && toDarkTheme) ||
                (theme.theme == theme.TYPES.LIGHT && !toDarkTheme)) {
            return;
        }
        
        if (toDarkTheme) {
            theme.enableDarkTheme();
        } else {
            theme.disableDarkTheme();
        }

        theme.themeChangedEvent.dispatch({ theme: theme.theme });
    };

    theme.enableDarkTheme = function () {
        theme.theme = theme.TYPES.DARK;
        document.getElementsByTagName("html")[0].dataset.bsTheme = "dark";
    };

    theme.disableDarkTheme = function () {
        theme.theme = theme.TYPES.LIGHT;
        document.getElementsByTagName("html")[0].dataset.bsTheme = "light";
    };

    window.theme = theme;
}

export { theme };
