import { MoveSuggestion } from "./MoveSuggestion";
import { settings } from "../utils/settings";
import { trainerG } from "../utils/trainerG";
import { gameplay } from "../gameplay";

class MoveSuggestionList {
    static ENCODE_ANALYZE_MOVE_INDICATOR = -2;

    suggestions = [];
    analyzeMoveSuggestion;
    passSuggestion;
    visits;
    playIndex;


    constructor(suggestions, analyzeMoveSuggestion) {
        this.suggestions = suggestions ? suggestions : [];
        this.analyzeMoveSuggestion = analyzeMoveSuggestion;
    }


    add(suggestion) {
        this.suggestions.push(suggestion);
    }

    getFilterByWeaker() {
        let move = trainerG.board.get().move;
        if (!move) return this.suggestions;

        let playedCoord = new Coord(move.x, move.y);

        if (!settings.hideWeakerOptions || gameplay.chosenNotPlayedCoordHistory.get() || !this.find(playedCoord)) {
            return this.suggestions;
        }

        let index;
        let playedCoordIndex;
        for (index = 0; index < this.suggestions.length; index++) {
            if (playedCoordIndex == null) {
                if (this.suggestions[index].coord.compare(playedCoord)) {
                    playedCoordIndex = index;
                }
            } else {
                if (this.suggestions[index].visits != this.suggestions[playedCoordIndex].visits) {
                    index--;
                    break;
                }
            }
        }

        return this.suggestions.slice(0, index + 1);
    }

    find(coord) {
        for (const suggestion of this.suggestions) {
            if (suggestion.coord.compare(coord)) {
                return suggestion;
            }
        }
    }

    encode() {
        let encoded = [];

        if (this.analyzeMoveSuggestion) {
            encoded = byteUtils.numToBytes(this.ENCODE_ANALYZE_MOVE_INDICATOR, 2, encoded);
            encoded = encoded.concat(this.analyzeMoveSuggestion.encode());
        }

        encoded = byteUtils.numToBytes(this.suggestions.length, 2, encoded);

        for (const suggestion of this.suggestions) {
            encoded = encoded.concat(suggestion.encode());
        }

        return encoded;
    }


    get(index) {
        return this.suggestions[index];
    }

    set(index, suggestion) {
        this.suggestions[index] = suggestion;
    }

    length() {
        return this.suggestions.length;
    }


    static fromKataGo(kataGoSuggestions) {
        let suggestions = new MoveSuggestionList();
        suggestions.passSuggestion = kataGoSuggestions.passSuggestion;
        suggestions.visits = kataGoSuggestions.visits;
        suggestions.playIndex = kataGoSuggestions.playIndex;

        for (const kataGoSuggestion of kataGoSuggestions.suggestions) {
            suggestions.add(MoveSuggestion.fromKataGo(kataGoSuggestion));
        }

        return suggestions;
    }

    static fromServer(serverSuggestions) {
        let suggestions = new MoveSuggestionList(serverSuggestions.analyzeMoveSuggestion);
        suggestions.passSuggestion = serverSuggestions.passSuggestion;
        suggestions.visits = serverSuggestions.visits;
        suggestions.playIndex = serverSuggestions.playIndex;

        for (const serverSuggestion of serverSuggestions.suggestions) {
            suggestions.add(MoveSuggestion.fromKataGo(serverSuggestion));
        }

        return suggestions;
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.MoveSuggestionList) window.trainer.MoveSuggestionList = MoveSuggestionList;

export { MoveSuggestionList };
