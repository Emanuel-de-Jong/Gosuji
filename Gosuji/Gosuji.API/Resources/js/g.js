let g = window.g;

if (typeof g === "undefined") {
    g = { id: "g" };

    g.VERSION = "0.3";
    g.DEBUG = true;
    g.LOG = false;

    g.COLOR_TYPE = {
        B: -1,
        RANDOM: 0,
        W: 1,
    };

    g.COLOR_NAME_TYPE = {
        B: "B",
        RANDOM: "R",
        W: "W",
    };

    g.COLOR_FULL_NAME_TYPE = {
        B: "Black",
        RANDOM: "Random",
        W: "White",
    };


    g.getIsDebug = () => g.DEBUG;

    g.colorNumToName = function (num) {
        return num == g.COLOR_TYPE.W ? g.COLOR_NAME_TYPE.W : g.COLOR_NAME_TYPE.B;
    };

    g.colorNumToFullName = function (num) {
        return num == g.COLOR_TYPE.W ? g.COLOR_FULL_NAME_TYPE.W : g.COLOR_FULL_NAME_TYPE.B;
    };

    g.colorNameToNum = function (name) {
        return name == g.COLOR_NAME_TYPE.W ? g.COLOR_TYPE.W : g.COLOR_TYPE.B;
    };

    g.colorFullNameToNum = function (name) {
        return name == g.COLOR_FULL_NAME_TYPE.W ? g.COLOR_TYPE.W : g.COLOR_TYPE.B;
    };

    window.g = g;
}

export { g };
