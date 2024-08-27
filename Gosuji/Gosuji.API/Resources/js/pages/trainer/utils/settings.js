import { sgf } from "./sgf";
import { trainerG } from "./trainerG";

let settings = { id: "settings" };


settings.SETTINGS = {
    boardsize: utils.TYPE.INT,
    handicap: utils.TYPE.INT,
    colorType: utils.TYPE.INT,
    preMovesSwitch: utils.TYPE.BOOL,
    preMoves: utils.TYPE.INT,
    preVisits: utils.TYPE.INT,
    selfplayVisits: utils.TYPE.INT,
    suggestionVisits: utils.TYPE.INT,
    opponentVisits: utils.TYPE.INT,
    wrongMoveCorrection: utils.TYPE.BOOL,

    ruleset: utils.TYPE.STRING,
    komiChangeStyle: utils.TYPE.STRING,
    komi: utils.TYPE.FLOAT,

    preOptions: utils.TYPE.INT,
    preOptionPerc: utils.TYPE.FLOAT,
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

    suggestionOptions: utils.TYPE.INT,
    hideOptions: utils.TYPE.STRING,
    hideWeakerOptions: utils.TYPE.BOOL,
    minVisitsPercSwitch: utils.TYPE.BOOL,
    minVisitsPerc: utils.TYPE.FLOAT,
    maxVisitDiffPercSwitch: utils.TYPE.BOOL,
    maxVisitDiffPerc: utils.TYPE.FLOAT,

    opponentOptionsSwitch: utils.TYPE.BOOL,
    opponentOptions: utils.TYPE.INT,
    opponentOptionPerc: utils.TYPE.FLOAT,
    showOpponentOptions: utils.TYPE.BOOL,
    
    selfplayPlaySpeed: utils.TYPE.FLOAT,
};

settings.HIDE_OPTIONS = {
    NEVER: "NEVER",
    PERFECT: "PERFECT",
    RIGHT: "RIGHT",
    ALWAYS: "ALWAYS",
};


settings.init = function (gameLoadInfo) {
    for (const name of Object.keys(settings.SETTINGS)) {
        settings[name + "Element"] = document.getElementById(name);
    }

    utils.addEventListeners(
        utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"]),
        "input",
        settings.inputAndSelectInputListener
    );
    settings.komiChangeStyleElement.addEventListener("input", settings.komiChangeStyleElementInputListener);
    settings.handicapElement.addEventListener("input", settings.setKomi);
    settings.rulesetElement.addEventListener("input", settings.setKomi);
    settings.boardsizeElement.addEventListener("input", settings.setKomi);

    utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"]).forEach((input) => {
        if (input.type != "checkbox") {
            input.required = true;
        }
        if (utils.getSiblingByClass(input, "form-invalid-message") == null) {
            input.insertAdjacentHTML("afterend", '<div class="form-invalid-message"></div>');
        }
    });

    for (const name of Object.keys(settings.SETTINGS)) {
        settings.updateSetting(name);
    }

    settings.clear(gameLoadInfo);
};

settings.clear = function (gameLoadInfo) {
    trainerG.setColor(gameLoadInfo ? gameLoadInfo.color : settings.colorType);
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

settings.komiChangeStyleElementInputListener = function () {
    if (settings.komiChangeStyle == "Automatic") {
        settings.komiElement.disabled = true;
        settings.setKomi();
    } else {
        settings.komiElement.disabled = false;
    }
};

settings.setKomi = function () {
    if (settings.komiChangeStyle != "Automatic") return;

    let oldKomi = settings.komi;
    let komi;

    if (settings.handicap != 0) {
        komi = 0.5;
    } else {
        if (settings.ruleset == "Japanese") {
            switch (settings.boardsize) {
                case 19:
                    komi = 6.5;
                    break;
                case 13:
                    komi = 6.5;
                    break;
                case 9:
                    komi = 6.5;
                    break;
            }
        } else if (settings.ruleset == "Chinese") {
            switch (settings.boardsize) {
                case 19:
                    komi = 7.5;
                    break;
                case 13:
                    komi = 6.5;
                    break;
                case 9:
                    komi = 6.5;
                    break;
            }
        }
    }

    if (komi != oldKomi) {
        settings.setSetting("komi", komi);
    }
};

export { settings };
