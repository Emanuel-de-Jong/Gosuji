import { ratioChart } from "./ratioChart";
import { scoreChart } from "./scoreChart";
import { settings } from "./settings";
import { sgf } from "./sgf";
import { trainerG } from "./trainerG";

let sgfComment = { id: "sgfComment" };


sgfComment.init = function () {
    
};

sgfComment.clear = function () {
    sgfComment.setComment(trainerG.MOVE_TYPE.INIT);
};


sgfComment.setComment = function (moveType) {
    if (moveType == trainerG.MOVE_TYPE.NONE) return;

    let comment;
    switch (moveType) {
        case trainerG.MOVE_TYPE.INIT:
            comment = sgfComment.createInitComment();
            break;
        case trainerG.MOVE_TYPE.FORCED_CORNER:
            comment = sgfComment.createForcedCornerComment();
            break;
        case trainerG.MOVE_TYPE.PRE:
            comment = sgfComment.createPreComment();
            break;
        case trainerG.MOVE_TYPE.SELFPLAY:
            comment = sgfComment.createSelfplayComment();
            break;
        case trainerG.MOVE_TYPE.PLAYER:
            comment = sgfComment.createPlayerComment();
            break;
        case trainerG.MOVE_TYPE.OPPONENT:
            comment = sgfComment.createOpponentComment();
            break;
    }

    trainerG.board.editor.setComment(comment);

    trainerG.moveTypeHistory.add(moveType);
};

sgfComment.createInitComment = function () {
    return "GOSUJI " + g.VERSION +
        "\nBoard size: " + trainerG.board.boardsize +
        "\nHandicap: " + trainerG.board.handicap +
        "\nPre moves switch: " + settings.preMovesSwitch +
        "\nPre moves: " + settings.preMoves +
        "\nHide options: " + settings.hideOptions +
        "\nColor: " + g.colorNumToFullName(trainerG.color) +
        "\nSuggestion strength: " + settings.suggestionVisits +
        "\nOpponent strength: " + settings.opponentVisits +
        "\nWrong move correction: " + settings.wrongMoveCorrection +

        "\n\nGame" +
        "\nKomi change style: " + settings.komiChangeStyle +
        "\nKomi: " + sgf.komi +
        "\nRuleset: " + sgf.ruleset +

        "\n\nPre moves" +
        "\nForce opponent corners: " + settings.forceOpponentCorners +
        "\n4-4 switch: " + settings.cornerSwitch44 +
        "\n3-4 switch: " + settings.cornerSwitch34 +
        "\n3-3 switch: " + settings.cornerSwitch33 +
        "\n4-5 switch: " + settings.cornerSwitch45 +
        "\n3-5 switch: " + settings.cornerSwitch35 +
        "\n4-4 chance: " + settings.cornerChance44 +
        "\n3-4 chance: " + settings.cornerChance34 +
        "\n3-3 chance: " + settings.cornerChance33 +
        "\n4-5 chance: " + settings.cornerChance45 +
        "\n3-5 chance: " + settings.cornerChance35 +
        "\nPre move strength: " + settings.preVisits +
        "\nOptions: " + settings.preOptions +
        "\nOption chance switch: " + settings.preOptionPercSwitch +
        "\nOption chance: " + settings.preOptionPerc + "%" +

        "\n\nFilters" +
        "\nSuggestion options: " + settings.suggestionOptions +
        "\nHide weaker options: " + settings.hideWeakerOptions +
        "\nMin strength switch: " + settings.minVisitsPercSwitch +
        "\nMin strength: " + settings.minVisitsPerc + "%" +
        "\nMax strength difference switch: " + settings.maxVisitDiffPercSwitch +
        "\nMax strength difference: " + settings.maxVisitDiffPerc +

        "\n\nOpponent" +
        "\nOptions: " + settings.opponentOptions +
        "\nHide options: " + settings.hideOpponentOptions +
        "\nOption chance switch: " + settings.opponentOptionPercSwitch +
        "\nOption chance: " + settings.opponentOptionPerc + "%" +

        "\n\nSelfplay" +
        "\nPlay speed: " + settings.selfplayPlaySpeed +
        "\nSelfplay strength: " + settings.selfplayVisits +

        "\n" + sgfComment.createCommentKataGo();
};

sgfComment.createForcedCornerComment = function () {
    return "FORCED CORNER MOVE" +
        sgfComment.createCommentScore();
};

sgfComment.createPreComment = function () {
    return "PRE MOVE" +
        sgfComment.createCommentVisits() +
        "\n" + sgfComment.createCommentScore();
};

sgfComment.createSelfplayComment = function () {
    return "SELFPLAY MOVE" +
        sgfComment.createCommentVisits() +
        "\n" + sgfComment.createCommentScore();
};

sgfComment.createPlayerComment = function () {
    return "PLAYER MOVE" +
        sgfComment.createCommentVisits() +
        "\n" + sgfComment.createCommentRatio() +
        "\n" + sgfComment.createCommentScore();
};

sgfComment.createOpponentComment = function () {
    return "OPPONENT MOVE" +
        sgfComment.createCommentVisits() +
        "\n" + sgfComment.createCommentRatio() +
        "\n" + sgfComment.createCommentScore();
};


sgfComment.createCommentVisits = function () {
    let suggestions = trainerG.suggestions;
    if (suggestions == null) return "";

    let comment = "\nVisits";
    for (let i = 0; i < suggestions.length(); i++) {
        let suggestion = suggestions.get(i);
        if (i != 0 && suggestion.visits == suggestions.get(i - 1).visits) continue;

        comment += "\n" + suggestion.grade + ": " + suggestion.visits;
    }

    return comment;
};

sgfComment.createCommentRatio = function () {
    if (!ratioChart.chart || ratioChart.rights.length == 0) return "";

    return "\nRight ratio: " + ratioChart.rights[ratioChart.rights.length - 1] + "%" +
        "\nPerfect(A) ratio: " + ratioChart.perfects[ratioChart.perfects.length - 1] + "%";
}

sgfComment.createCommentScore = function () {
    let score = scoreChart.history.get();
    if (score == null) return "";

    return "\nScore " + g.colorNumToName(scoreChart.colorElement.value) +
        "\nWinrate: " + score.formatWinrate() + "%" +
        "\nScore: " + score.formatScoreLead();
};

sgfComment.createCommentKataGo = function () {
    if (trainerG.kataGoVersion == null) return "";

    return "\nKataGo" +
        "\nVersion: " + trainerG.kataGoVersion.version +
        "\nModel: " + trainerG.kataGoVersion.model;
};

export { sgfComment };
