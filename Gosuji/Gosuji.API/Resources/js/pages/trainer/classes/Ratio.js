class Ratio {
    moveNumber;
    total;

    right;
    perfect;

    constructor(moveNumber, total, right, perfect) {
        this.moveNumber = moveNumber;
        this.total = total;
        this.right = right;
        this.perfect = perfect;
    }


    getRightPercent() {
        return this.getPercent(this.right);
    }

    getPerfectPercent() {
        return this.getPercent(this.perfect);
    }

    getPercent(count) {
        return Math.round((count / this.total) * 100);
    }
}

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.Ratio) window.trainer.Ratio = Ratio;

export { Ratio };
