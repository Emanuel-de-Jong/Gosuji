import { kataGo } from "./utils/kataGo";
import { settings } from "./utils/settings";
import { trainerG } from "./utils/trainerG";

let cornerPlacer = { id: "cornerPlacer" };


cornerPlacer.CORNERS = {
    19: [
        { c44: { x: 4, y: 4 }, c34: { x: 3, y: 4 }, c43: { x: 4, y: 3 }, c33: { x: 3, y: 3 }, c45: { x: 4, y: 5 }, c54: { x: 5, y: 4 }, c35: { x: 3, y: 5 }, c53: { x: 5, y: 3 } },
        { c44: { x: 16, y: 4 }, c34: { x: 17, y: 4 }, c43: { x: 16, y: 3 }, c33: { x: 17, y: 3 }, c45: { x: 16, y: 5 }, c54: { x: 15, y: 4 }, c35: { x: 17, y: 5 }, c53: { x: 15, y: 3 } },
        { c44: { x: 4, y: 16 }, c34: { x: 3, y: 16 }, c43: { x: 4, y: 17 }, c33: { x: 3, y: 17 }, c45: { x: 4, y: 15 }, c54: { x: 5, y: 16 }, c35: { x: 3, y: 15 }, c53: { x: 5, y: 17 } },
        { c44: { x: 16, y: 16 }, c34: { x: 17, y: 16 }, c43: { x: 16, y: 17 }, c33: { x: 17, y: 17 }, c45: { x: 16, y: 15 }, c54: { x: 15, y: 16 }, c35: { x: 17, y: 15 }, c53: { x: 15, y: 17 } },
    ],
    13: [
        { c44: { x: 4, y: 4 }, c34: { x: 3, y: 4 }, c43: { x: 4, y: 3 }, c33: { x: 3, y: 3 }, c45: { x: 4, y: 5 }, c54: { x: 5, y: 4 }, c35: { x: 3, y: 5 }, c53: { x: 5, y: 3 } },
        { c44: { x: 10, y: 4 }, c34: { x: 11, y: 4 }, c43: { x: 10, y: 3 }, c33: { x: 11, y: 3 }, c45: { x: 10, y: 5 }, c54: { x: 9, y: 4 }, c35: { x: 11, y: 5 }, c53: { x: 9, y: 3 } },
        { c44: { x: 4, y: 10 }, c34: { x: 3, y: 10 }, c43: { x: 4, y: 11 }, c33: { x: 3, y: 11 }, c45: { x: 4, y: 9 }, c54: { x: 5, y: 10 }, c35: { x: 3, y: 9 }, c53: { x: 5, y: 11 } },
        { c44: { x: 10, y: 10 }, c34: { x: 11, y: 10 }, c43: { x: 10, y: 11 }, c33: { x: 11, y: 11 }, c45: { x: 10, y: 9 }, c54: { x: 9, y: 10 }, c35: { x: 11, y: 9 }, c53: { x: 9, y: 11 } },
    ],
};


cornerPlacer.init = function () {
    cornerPlacer.clear();
};

cornerPlacer.clear = function () { };


cornerPlacer.shouldForce = function (isAfterDraw = false) {
    let moveNumber = trainerG.board.getMoveNumber();
    if (isAfterDraw) {
        moveNumber--;
    }

    if (moveNumber > 4) return false;

    if (trainerG.board.handicap != 0 || (trainerG.board.boardsize != 19 && trainerG.board.boardsize != 13)) return false;

    if ((settings.forceOpponentCorners == settings.FORCE_OPPONENT_CORNERS.FIRST ||
        settings.forceOpponentCorners == settings.BOTH) && (
            trainerG.color == g.COLOR_TYPE.W && moveNumber == 0 ||
            trainerG.color == g.COLOR_TYPE.B && moveNumber == 1) ||
        (settings.forceOpponentCorners == settings.FORCE_OPPONENT_CORNERS.SECOND ||
        settings.forceOpponentCorners == settings.FORCE_OPPONENT_CORNERS.BOTH) && (
            trainerG.color == g.COLOR_TYPE.W && moveNumber == 2 ||
            trainerG.color == g.COLOR_TYPE.B && moveNumber == 3)) {
        return true;
    }

    return false;
};

cornerPlacer.play = async function () {
    let cornerOptions = cornerPlacer.getEmptyCorner();
    let coord = cornerPlacer.chooseCornerOption(cornerOptions);
    let suggestion = await kataGo.playForcedCorner(coord);
    await trainerG.board.play(suggestion, trainerG.MOVE_ORIGIN.FORCED_CORNER);
};

cornerPlacer.getEmptyCorner = function () {
    let corners = utils.shuffleArray(cornerPlacer.CORNERS[settings.boardsize]);

    let stoneFound = false;
    for (const cornerOptions of corners) {
        for (const option of Object.values(cornerOptions)) {
            if (trainerG.board.findStone(option)) {
                stoneFound = true;
                break;
            }
        }

        if (!stoneFound) {
            return cornerOptions;
        } else {
            stoneFound = false;
        }
    }
};

cornerPlacer.chooseCornerOption = function (cornerOptions) {
    let totalCornerChance =
        (settings.cornerSwitch44 ? settings.cornerChance44 : 0) +
        (settings.cornerSwitch34 ? settings.cornerChance34 : 0) +
        (settings.cornerSwitch33 ? settings.cornerChance33 : 0) +
        (settings.cornerSwitch45 ? settings.cornerChance45 : 0) +
        (settings.cornerSwitch35 ? settings.cornerChance35 : 0);

    let coord;
    let cornerTypeRange = 0;
    let rndCornerType = utils.randomInt(totalCornerChance);
    let rndCornerSide = utils.randomInt(2);
    if (
        settings.cornerSwitch44 &&
        rndCornerType < (cornerTypeRange = cornerTypeRange + settings.cornerChance44)
    ) {
        coord = cornerOptions.c44;
    } else if (
        settings.cornerSwitch34 &&
        rndCornerType < (cornerTypeRange = cornerTypeRange + settings.cornerChance34)
    ) {
        coord = rndCornerSide ? cornerOptions.c34 : cornerOptions.c43;
    } else if (
        settings.cornerSwitch33 &&
        rndCornerType < (cornerTypeRange = cornerTypeRange + settings.cornerChance33)
    ) {
        coord = cornerOptions.c33;
    } else if (
        settings.cornerSwitch45 &&
        rndCornerType < (cornerTypeRange = cornerTypeRange + settings.cornerChance45)
    ) {
        coord = rndCornerSide ? cornerOptions.c45 : cornerOptions.c54;
    } else {
        coord = rndCornerSide ? cornerOptions.c35 : cornerOptions.c53;
    }

    return coord;
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.cornerPlacer) window.trainer.cornerPlacer = cornerPlacer;

export { cornerPlacer };
