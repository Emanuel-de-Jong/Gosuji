import { kataGo } from "./utils/kataGo";
import { scoreChart } from "./utils/scoreChart";
import { settings } from "./utils/settings";
import { sgfComment } from "./utils/sgfComment";
import { sgf } from "./utils/sgf";
import { stats } from "./utils/stats";
import { trainerG } from "./utils/trainerG";
import { cornerPlacer } from "./cornerPlacer";
import { gameplay } from "./gameplay";
import { trainerPage } from "./trainer";
import { preMovePlacer } from "./preMovePlacer";
import { selfplay } from "./selfplay";

let debug = { id: "debug" };

// 0: none
// 1: tree
// 2: full game
// 3: full game except last move and pass
debug.testData = 0;


debug.init = function () {
    // debug.logAllFuncCalls();

    debug.clear();
    debug.isInitialized = true;
};

debug.clear = function () { };


debug.logAllFuncCalls = function () {
    let objs = [
        byteUtils,
        custom,
        G,
        utils,

        kataGo,
        scoreChart,
        settings,
        sgf,
        sgfComment,
        stats,
        trainerG,

        cornerPlacer,
        gameplay,
        trainerPage,
        preMovePlacer,
        selfplay,
    ];

    for (let i = 0; i < objs.length; i++) {
        let obj = objs[i];
        let funcNames = Object.getOwnPropertyNames(obj).filter((item) => typeof obj[item] === "function");
        for (let j = 0; j < funcNames.length; j++) {
            let funcName = funcNames[j];

            obj[funcName] = (function () {
                let cachedFunc = obj[funcName];

                return function () {
                    let log = obj.id + "." + funcName + "(";
                    for (let x = 0; x < arguments.length; x++) {
                        let argument = arguments[x];
                        switch (typeof argument) {
                            case "object":
                                log += "obj";
                                break;
                            case "function":
                                log += "func";
                                break;
                            default:
                                log += argument;
                        }

                        if (x != arguments.length - 1) {
                            log += ", ";
                        }
                    }
                    log += ")";
                    console.log(log);

                    let result = cachedFunc.apply(this, arguments);
                    return result;
                };
            })();
        }
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.debug) window.trainer.debug = debug;

export { debug };
