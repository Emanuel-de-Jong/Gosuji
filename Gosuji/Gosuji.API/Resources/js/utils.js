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


    utils.downloadByteFile = function (name, extension, bytes, type, encoding="utf-8") {
        let byteArray = new Uint8Array(bytes);
        utils.downloadTextFile(name, extension, byteArray, type, encoding);
    };

    utils.downloadTextFile = function (name, extension, text, type, encoding="utf-8") {
        if (window.electronAPI) {
            window.electronAPI.saveFile(name, text);
        } else {
            let blob = new Blob([text], { encoding: encoding, type: type });
            let url = URL.createObjectURL(blob);
        
            let anchor = document.createElement("a");
            anchor.href = url;
            anchor.download = name + "." + extension;
            anchor.click();
            
            URL.revokeObjectURL(url);
        }
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
        for (let c of ca) {
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


    utils.sleep = function (ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    };


    utils.classToObject = function (classInstance) {
        if (typeof classInstance !== 'object' || classInstance === null) {
            return classInstance;
        }
    
        if (Array.isArray(classInstance)) {
            return classInstance.map(item => utils.classToObject(item));
        }
    
        const obj = {};
        for (const key of Object.getOwnPropertyNames(classInstance)) {
            if (classInstance.hasOwnProperty(key)) {
                const value = classInstance[key];
                if (typeof value === 'object' && value !== null) {
                    obj[key] = utils.classToObject(value);
                } else {
                    obj[key] = value;
                }
            }
        }
    
        return obj;
    };

    utils.deepCopyObject = function (obj) {
        let clone = Array.isArray(obj) ? [] : {};
        for (const [key, val] of Object.entries(obj)) {
            if (typeof val === "object" && val !== null) {
                clone[key] = utils.deepCopyObject(val);
            } else {
                clone[key] = val;
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

    utils.randomString = function (length = 10) {
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
        let result = '';
        for (let i = 0; i < length; i++) {
            result += chars[utils.randomInt(chars.length)];
        }
        return result;
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
        for (const event of events) {
            element.addEventListener(event, fn);
        }
    };

    utils.addEventListeners = function (elements, event, fn) {
        for (const element of elements) {
            element.addEventListener(event, fn);
        }
    };

    utils.addEventsListeners = function (elements, events, fn) {
        for (const event of events) {
            for (const element of elements) {
                element.addEventListener(event, fn);
            }
        }
    };

    utils.querySelectorAlls = function (selectors) {
        let elementArrays = [];
        for (const selector of selectors) {
            elementArrays.push(Array.from(document.querySelectorAll(selector)));
        }
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
