let G = window.G;

if (typeof G === "undefined") {
    G = {};

    G.VERSION = "0.3";
    G.LOG = false;

    G.COLOR_TYPE = {
        B: -1,
        RANDOM: 0,
        W: 1,
    };

    G.COLOR_NAME_TYPE = {
        B: "B",
        RANDOM: "R",
        W: "W",
    };

    G.COLOR_FULL_NAME_TYPE = {
        B: "Black",
        RANDOM: "Random",
        W: "White",
    };


    G.colorNumToName = function (num) {
        return num == G.COLOR_TYPE.W ? G.COLOR_NAME_TYPE.W : G.COLOR_NAME_TYPE.B;
    };

    G.colorNumToFullName = function (num) {
        return num == G.COLOR_TYPE.W ? G.COLOR_FULL_NAME_TYPE.W : G.COLOR_FULL_NAME_TYPE.B;
    };

    G.colorNameToNum = function (name) {
        return name == G.COLOR_NAME_TYPE.W ? G.COLOR_TYPE.W : G.COLOR_TYPE.B;
    };

    G.colorFullNameToNum = function (name) {
        return name == G.COLOR_FULL_NAME_TYPE.W ? G.COLOR_TYPE.W : G.COLOR_TYPE.B;
    };

    window.G = G;
}

export { G };
