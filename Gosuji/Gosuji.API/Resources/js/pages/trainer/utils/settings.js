import { sgf } from "./sgf";
import { trainerG } from "./trainerG";

let settings = { id: "settings" };


settings.SETTINGS = {
    boardsize: utils.TYPE.INT,
    handicap: utils.TYPE.INT,
    preMovesSwitch: utils.TYPE.BOOL,
    preMoves: utils.TYPE.INT,
    hideOptions: utils.TYPE.STRING,
    colorType: utils.TYPE.INT,
    suggestionVisits: utils.TYPE.INT,
    opponentVisits: utils.TYPE.INT,
    wrongMoveCorrection: utils.TYPE.BOOL,

    customKomi: utils.TYPE.BOOL,
    komi: utils.TYPE.FLOAT,
    ruleset: utils.TYPE.STRING,

    forceOpponentCorners: utils.TYPE.STRING,
    cornerSwitch44: utils.TYPE.BOOL,
    cornerSwitch34: utils.TYPE.BOOL,
    cornerSwitch33: utils.TYPE.BOOL,
    cornerSwitch45: utils.TYPE.BOOL,
    cornerSwitch35: utils.TYPE.BOOL,
    cornerChance44: utils.TYPE.INT,
    cornerChance34: utils.TYPE.INT,
    cornerChance33: utils.TYPE.INT,
    cornerChance45: utils.TYPE.INT,
    cornerChance35: utils.TYPE.INT,
    preVisits: utils.TYPE.INT,
    preOptions: utils.TYPE.INT,
    preOptionPercSwitch: utils.TYPE.BOOL,
    preOptionPerc: utils.TYPE.FLOAT,

    suggestionOptions: utils.TYPE.INT,
    hideWeakerOptions: utils.TYPE.BOOL,
    minVisitsPercSwitch: utils.TYPE.BOOL,
    minVisitsPerc: utils.TYPE.FLOAT,
    maxVisitDiffPercSwitch: utils.TYPE.BOOL,
    maxVisitDiffPerc: utils.TYPE.FLOAT,

    opponentOptions: utils.TYPE.INT,
    hideOpponentOptions: utils.TYPE.STRING,
    opponentOptionPercSwitch: utils.TYPE.BOOL,
    opponentOptionPerc: utils.TYPE.FLOAT,
    
    selfplayPlaySpeed: utils.TYPE.FLOAT,
    selfplayVisits: utils.TYPE.INT,
};

settings.PRE_GAME_SETTINGS = [
    "boardsize",
    "handicap",
    "preMovesSwitch",
    "preMoves",
    "colorType",
    "customKomi",
    "komi",
    "ruleset",
    "forceOpponentCorners",
    "cornerSwitch44", "cornerSwitch34", "cornerSwitch33", "cornerSwitch45", "cornerSwitch35",
    "cornerChance44", "cornerChance34", "cornerChance33", "cornerChance45", "cornerChance35",
    "preVisits",
    "preOptions",
    "preOptionPercSwitch",
    "preOptionPerc",
];

settings.HIDE_OPTIONS = {
    NEVER: "NEVER",
    PERFECT: "PERFECT",
    RIGHT: "RIGHT",
    ALWAYS: "ALWAYS",
};

settings.HIDE_OPPONENT_OPTIONS = {
    NEVER: "NEVER",
    PERFECT: "PERFECT",
    ALWAYS: "ALWAYS",
};


settings.init = function (gameLoadInfo) {
    for (const key in settings.SETTINGS) {
        settings[key + "Element"] = document.getElementById(key);
    }

    utils.addEventListeners(
        utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"]),
        "input",
        settings.inputAndSelectInputListener
    );
    settings.customKomiElement.addEventListener("input", settings.toggleCustomKomi);
    settings.handicapElement.addEventListener("input", settings.setKomi);
    settings.rulesetElement.addEventListener("input", settings.setRuleset);
    settings.boardsizeElement.addEventListener("input", settings.setKomi);

    for (const input of utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"])) {
        if (input.type != "checkbox") {
            input.required = true;
        }
        if (utils.getSiblingByClass(input, "form-invalid-message") == null) {
            input.insertAdjacentHTML("afterend", '<div class="form-invalid-message"></div>');
        }
    }

    for (const key in settings.SETTINGS) {
        settings.updateSetting(key);
    }

    settings.clear(gameLoadInfo);
};

settings.clear = function (gameLoadInfo) {
    trainerG.setColor(gameLoadInfo ? gameLoadInfo.color : settings.colorType);

    if (gameLoadInfo) {
        settings.togglePreGameSettings();
    }
};


settings.updateSetting = function (name) {
    let type = settings.SETTINGS[name];

    let element = settings[name + "Element"];

    let value = type == utils.TYPE.BOOL ? element.checked : element.value;
    if (type == utils.TYPE.INT) {
        value = parseInt(value);
    } else if (type == utils.TYPE.FLOAT) {
        value = parseFloat(value);
    }

    settings[name] = value;
};

settings.setSetting = function (name, value) {
    settings[name + "Element"].value = value;
    settings[name + "Element"].dispatchEvent(new Event("input"));
};

settings.inputAndSelectInputListener = function (event) {
    let element = event.target;
    if (settings.validateInput(element)) {
        settings.updateSetting(element.id);
    }
};

settings.togglePreGameSettings = function (enable = false) {
    for (const name of settings.PRE_GAME_SETTINGS) {
        settings[name + "Element"].disabled = !enable;
    }
};

settings.validateInput = function (input) {
    let valid = input.validity.valid;
    if (valid) {
        settings.hideInvalidMessage(input);
    } else {
        settings.showInvalidMessage(input);
    }
    return valid;
};

settings.showInvalidMessage = function (input) {
    input.classList.add("form-invalid");

    let messageDiv = utils.getSiblingByClass(input, "form-invalid-message");
    messageDiv.textContent = input.validationMessage;
};

settings.hideInvalidMessage = function (input) {
    input.classList.remove("form-invalid");

    let messageDiv = utils.getSiblingByClass(input, "form-invalid-message");
    messageDiv.textContent = "";
};

settings.toggleCustomKomi = async function () {
    if (settings.customKomi) {
        settings.komiElement.disabled = false;
    } else {
        settings.komiElement.disabled = true;
        await settings.setKomi();
    }
};

settings.setRuleset = async function () {
    await sgf.setRuleset(settings.ruleset);
    await settings.setKomi();
};

settings.setKomi = async function () {
    if (settings.customKomi) return;

    let oldKomi = settings.komi;
    let komi = await trainerG.trainerRef.invokeMethodAsync("GetDefaultKomi", settings.ruleset);

    if (komi != oldKomi) {
        settings.setSetting("komi", komi);
        await sgf.setKomi(komi);
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.settings) window.trainer.settings = settings;

export { settings };
