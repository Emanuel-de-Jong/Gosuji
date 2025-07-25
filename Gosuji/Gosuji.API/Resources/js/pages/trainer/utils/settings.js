import { kataGo } from "./kataGo";
import { sgf } from "./sgf";
import { trainerG } from "./trainerG";

let settings = { id: "settings" };


settings.SETTINGS = {
    boardsize: utils.TYPE.INT,
    handicap: utils.TYPE.INT,
    preMovesSwitch: utils.TYPE.BOOL,
    preMoves: utils.TYPE.INT,
    hideOptions: utils.TYPE.INT,
    colorType: utils.TYPE.INT,
    suggestionVisits: utils.TYPE.INT,
    opponentVisits: utils.TYPE.INT,
    wrongMoveCorrection: utils.TYPE.BOOL,

    customKomi: utils.TYPE.BOOL,
    komi: utils.TYPE.FLOAT,
    ruleset: utils.TYPE.STRING,

    forceOpponentCorners: utils.TYPE.INT,
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
    hideOpponentOptions: utils.TYPE.INT,
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
    NEVER: 0,
    PERFECT: 1,
    RIGHT: 2,
    ALWAYS: 3,
};

settings.HIDE_OPPONENT_OPTIONS = {
    NEVER: 0,
    PERFECT: 1,
    ALWAYS: 3,
};

settings.FORCE_OPPONENT_CORNERS = {
    NONE: 0,
    FIRST: 1,
    SECOND: 2,
    BOTH: 3,
};


settings.init = async function (trainerSettingConfig) {
    for (const key in settings.SETTINGS) {
        settings[key + "Element"] = document.getElementById(key);
    }

    utils.addEventListeners(
        utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"]),
        "input",
        settings.inputAndSelectInputListener
    );
    settings.boardsizeElement.addEventListener("input", settings.setBoardsize);
    settings.colorTypeElement.addEventListener("input", settings.setColorType);
    settings.customKomiElement.addEventListener("input", settings.toggleCustomKomi);
    settings.handicapElement.addEventListener("input", settings.setKomi);
    settings.rulesetElement.addEventListener("input", settings.setRuleset);
    for (const input of utils.querySelectorAlls(["#settingsAccordion input", "#settingsAccordion select"])) {
        if (input.type != "checkbox") {
            input.required = true;
        }
        if (utils.getSiblingByClass(input, "form-invalid-message") == null) {
            input.insertAdjacentHTML("afterend", '<div class="form-invalid-message"></div>');
        }
    }

    await settings.syncWithCS(trainerSettingConfig);

    settings.clear();
    settings.isInitialized = true;
};

settings.clear = function () {
    trainerG.setColor(trainerG.gameLoadInfo ? trainerG.gameLoadInfo.color : settings.colorType);

    if (trainerG.gameLoadInfo) {
        settings.togglePreGameSettings();
    }
};


settings.updateSetting = async function (name) {
    let type = settings.SETTINGS[name];

    let element = settings[name + "Element"];

    let value = type == utils.TYPE.BOOL ? element.checked : element.value;
    if (type == utils.TYPE.INT) {
        value = parseInt(value);
    } else if (type == utils.TYPE.FLOAT) {
        value = parseFloat(value);
    }

    settings[name] = value;

    if (!settings.isSyncingWithCS) {
        if (name == "customKomi") {
            return;
        }

        const propertyName = name[0].toUpperCase() + name.slice(1);
        const strValue = type == utils.TYPE.BOOL ? '' + element.checked : element.value;
        await settings.updateTrainerSettingConfig(propertyName, strValue);
    }
};

settings.setSetting = async function (name, value, isBatchSet = false) {
    const elName = name + "Element";
    if (!settings.hasOwnProperty(elName)) {
        return;
    }

    const el = settings[elName];

    const type = settings.SETTINGS[name];
    if (type == utils.TYPE.BOOL) {
        el.checked = value;
    } else {
        el.value = value;
    }

    if (!isBatchSet) {
        el.dispatchEvent(new Event("input"));
    } else {
        await settings.updateSetting(name);
    }
};

settings.inputAndSelectInputListener = async function (event) {
    let element = event.target;
    if (settings.validateInput(element)) {
        await settings.updateSetting(element.id);
    }
};

settings.syncWithCS = async function (trainerSettingConfig) {
    settings.isSyncingWithCS = true;

    const customKomi = trainerSettingConfig.hasOwnProperty("komi") && trainerSettingConfig["komi"] != null;
    
    for (const [name, value] of Object.entries(trainerSettingConfig)) {
        await settings.setSetting(name, value, true);
    }

    await settings.setSetting("ruleset", trainerSettingConfig.getRuleset, true);
    await settings.setSetting("komi", trainerSettingConfig.getKomi, true);
    await settings.setSetting("suggestionVisits", trainerSettingConfig.getSuggestionVisits, true);
    await settings.setSetting("opponentVisits", trainerSettingConfig.getOpponentVisits, true);
    await settings.setSetting("preVisits", trainerSettingConfig.getPreVisits, true);
    await settings.setSetting("selfplayVisits", trainerSettingConfig.getSelfplayVisits, true);

    settings.customKomiElement.checked = customKomi;
    settings.customKomiElement.dispatchEvent(new Event("input"));

    settings.isSyncingWithCS = false;
};

settings.updateTrainerSettingConfig = async function (propertyName, value) {
    return await kataGo.invokeCS(trainerG.trainerRef, "UpdateTrainerSettingConfig", propertyName, value);
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

settings.setBoardsize = async function () {
    await trainerG.board.init();
};

settings.setColorType = async function () {
    trainerG.setColor(settings.colorType);
    await trainerG.board.init();
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
    sgf.setRuleset(settings.ruleset);
    await settings.setKomi();
};

settings.setKomi = async function () {
    if (settings.customKomi) return;

    let oldKomi = settings.komi;
    let komi = await trainerG.trainerRef.invokeMethodAsync("GetDefaultKomi", settings.ruleset);

    if (komi != oldKomi) {
        await settings.setSetting("komi", komi);
        sgf.setKomi(komi);
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.settings) window.trainer.settings = settings;

export { settings };
