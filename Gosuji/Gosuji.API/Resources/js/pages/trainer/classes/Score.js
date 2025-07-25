class Score {
    winrate;
    scoreLead;


    constructor(winrate, scoreLead) {
        this.winrate = winrate;
        this.scoreLead = scoreLead;
    }


    formatWinrate(shouldReverse = false) {
        return (shouldReverse ? this.getReverseWinrate() : this.winrate).toFixed(2);
    }

    formatScoreLead(shouldReverse = false) {
        return (shouldReverse ? this.getReverseScoreLead() : this.scoreLead).toFixed(1);
    }

    reverse() {
        this.winrate = this.getReverseWinrate();
        this.scoreLead = this.getReverseScoreLead();
        return this;
    }

    getReverseWinrate() {
        return 100 - this.winrate;
    }

    getReverseScoreLead() {
        return this.scoreLead * -1;
    }

    copy() {
        return new Score(this.winrate, this.scoreLead);
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.Score) window.trainer.Score = Score;

export { Score };
