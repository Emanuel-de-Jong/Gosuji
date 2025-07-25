let g = window.g;

if (typeof g === "undefined") {
    g = { id: "g" };

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

    g.MOVE_TYPE = {
        PASS: 0,
        ROOT: 1,
    };


    g.getResultStr = function (result) {
        if (result == 0) {
            return "Draw";
        }

        let color = g.COLOR_TYPE.B;
        if (result < 0) {
            color = g.COLOR_TYPE.W;
            result *= -1;
        }

        return g.colorNumToName(color) + "+" + result.toFixed(1);
    };


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
