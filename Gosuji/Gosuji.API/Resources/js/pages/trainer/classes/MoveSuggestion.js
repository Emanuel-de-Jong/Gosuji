import { Score } from "./Score";

class MoveSuggestion {
    coord;
    visits;
    score;
    grade;
    continuation;


    constructor(coord, visits, winrate, scoreLead, grade, continuation) {
        this.coord = coord;
        this.visits = visits;
        this.score = new Score(winrate, scoreLead);
        this.grade = grade;
        this.continuation = continuation;
    }


    isPass() {
        return this.coord.x == 0;
    }


    static fromKataGo(kataGoSuggestion) {
        return new MoveSuggestion(
            Coord.fromKataGo(kataGoSuggestion.coord),
            kataGoSuggestion.visits,
            kataGoSuggestion.score.winrate,
            kataGoSuggestion.score.scoreLead,
            kataGoSuggestion.grade,
            kataGoSuggestion.continuation
        );
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.MoveSuggestion) window.trainer.MoveSuggestion = MoveSuggestion;

export { MoveSuggestion };
