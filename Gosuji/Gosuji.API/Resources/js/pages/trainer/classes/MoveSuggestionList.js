import { MoveSuggestion } from "./MoveSuggestion";
import { settings } from "../utils/settings";
import { trainerG } from "../utils/trainerG";
import { gameplay } from "../gameplay";

class MoveSuggestionList {
    suggestions = [];
    analyzeMoveSuggestion;
    passSuggestion;
    visits;
    playIndex;

    constructor(suggestions, analyzeMoveSuggestion) {
        this.suggestions = suggestions ? suggestions : [];
        this.analyzeMoveSuggestion = analyzeMoveSuggestion;
    }

    getFilterByWeaker() {
        if (!settings.hideWeakerOptions || this.playIndex == null || gameplay.chosenNotPlayedCoordHistory.get()) {
            return this.suggestions;
        }

        let index;
        for (index = this.playIndex; index < this.suggestions.length; index++) {
            if (this.suggestions[index].grade != this.suggestions[this.playIndex].grade) {
                index--;
                break;
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

    add(suggestion) {
        this.suggestions.push(suggestion);
    }

    get(index = this.playIndex) {
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

        for (const kataGoSuggestion of kataGoSuggestions.suggestions) {
            suggestions.add(MoveSuggestion.fromKataGo(kataGoSuggestion));
        }

        return suggestions;
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.MoveSuggestionList) window.trainer.MoveSuggestionList = MoveSuggestionList;

export { MoveSuggestionList };
