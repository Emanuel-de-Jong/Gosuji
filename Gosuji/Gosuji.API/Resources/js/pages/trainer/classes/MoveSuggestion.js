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

    encode() {
        let encoded = [];
        encoded = encoded.concat(this.coord.encode());
        encoded = byteUtils.numToBytes(this.visits, 4, encoded);
        encoded = encoded.concat(this.score.encode());
        return encoded;
    }


    static fromKataGo(kataGoSuggestion) {
        return new MoveSuggestion(
            Coord.fromServer(kataGoSuggestion.coord),
            kataGoSuggestion.visits,
            kataGoSuggestion.score.winrate,
            kataGoSuggestion.score.scoreLead,
            kataGoSuggestion.grade,
            kataGoSuggestion.continuation
        );
    }

    static fromServer(serverSuggestion) {
        return new MoveSuggestion(
            Coord.fromServer(serverSuggestion.coord),
            serverSuggestion.visits,
            serverSuggestion.score.winrate,
            serverSuggestion.score.scoreLead,
            serverSuggestion.grade
        );
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.MoveSuggestion) window.trainer.MoveSuggestion = MoveSuggestion;

export { MoveSuggestion };
