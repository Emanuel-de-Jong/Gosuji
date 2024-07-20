let utils = window.utils;

if (typeof utils === "undefined") {
    utils = { id: "utils" };

    utils.TYPE = {
        NONE: 0,
        INT: 1,
        FLOAT: 2,
        BOOL: 3,
        STRING: 4,
    };


    utils.setLocal = function (name, value) {
        window.localStorage[name] = value;
    };

    utils.getLocal = function (name) {
        const value = window.localStorage[name];
        if (value === undefined || value === null || value === "") {
            return null;
        }
        return value;
    };


    utils.setCookie = function (cname, cvalue, exdays) {
        const d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        let expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    };

    utils.getCookie = function (cname) {
        let name = cname + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    };

    utils.deleteCookie = function (name) {
        utils.setCookie(name, "", {
            'max-age': -1
        });
    };


    utils.deepCopyObject = function (obj) {
        let clone = Array.isArray(obj) ? [] : {};
        for (let key in obj) {
            if (typeof obj[key] === "object" && obj[key] !== null) {
                clone[key] = utils.deepCopyObject(obj[key]);
            } else {
                clone[key] = obj[key];
            }
        }
        return clone;
    };


    utils.lazyLoadCSSLibrary = (url) => {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`link[href="${url}"]`)) {
                resolve();
                return;
            }

            const link = document.createElement("link");
            link.rel = "stylesheet"
            link.href = url;
            link.onload = () => {
                resolve();
            };
            link.onerror = (e) => {
                reject(e);
            };
            
            document.head.appendChild(link);
        });
    };

    utils.lazyLoadJSLibrary = (url) => {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`script[src="${url}"]`)) {
                resolve();
                return;
            }

            const script = document.createElement("script");
            script.src = url;
            script.onload = () => {
                resolve();
            };
            script.onerror = (e) => {
                reject(e);
            };

            document.head.appendChild(script);
        });
    };


    // min to (max-1)
    utils.randomInt = function (max, min = 0) {
        return Math.floor(Math.random() * (max - min)) + min;
    };


    utils.randomColor = function () {
        return [utils.randomInt(256), utils.randomInt(256), utils.randomInt(256), utils.randomInt(10) / 10.0];
    };

    utils.randomColorStr = function (hasAlpha = false) {
        let color = utils.randomColor();
        return "rgb" + (hasAlpha ? "a" : "") +
            "(" + color[0] + ", " + color[1] + ", " + color[2] + (hasAlpha ? ", " + color[3] : "") + ")";
    };


    utils.shuffleArray = function (array) {
        let currentIndex = array.length;
        let randomIndex;

        while (currentIndex != 0) {
            randomIndex = utils.randomInt(currentIndex);
            currentIndex--;
            [array[currentIndex], array[randomIndex]] = [array[randomIndex], array[currentIndex]];
        }

        return array;
    };


    utils.addEventsListener = function (element, events, fn) {
        events.forEach((event) => element.addEventListener(event, fn));
    };

    utils.addEventListeners = function (elements, event, fn) {
        elements.forEach((element) => element.addEventListener(event, fn));
    };

    utils.addEventsListeners = function (elements, events, fn) {
        events.forEach((event) => {
            elements.forEach((element) => {
                element.addEventListener(event, fn);
            });
        });
    };

    utils.querySelectorAlls = function (selectors) {
        let elementArrays = [];
        selectors.forEach((selector) => elementArrays.push(Array.from(document.querySelectorAll(selector))));
        return elementArrays.flat();
    };

    utils.getSiblingByClass = function (element, className) {
        let sibling = element.parentNode.firstElementChild;

        while (sibling) {
            if (sibling.classList.contains(className)) return sibling;

            sibling = sibling.nextElementSibling;
        }
    };

    window.utils = utils;
}

export { utils };
