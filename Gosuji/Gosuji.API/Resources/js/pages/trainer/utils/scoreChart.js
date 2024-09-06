import { History } from "../classes/History";
import { Score } from "../classes/Score";
import { stats } from "./stats";
import { trainerG } from "./trainerG";
import { debug } from "../debug";

let scoreChart = { id: "scoreChart" };


scoreChart.DATA = {
    datasets: [
        {
            label: "Winrate",
            pointRadius: 1,
            borderColor: "rgb(0, 255, 0)",
            backgroundColor: "rgba(0, 255, 0, 0.3)",
            // fill: {
            //     target: { value: 50 },
            // },
        },
        {
            label: "Score",
            yAxisID: "y1",
            pointRadius: 1,
            borderColor: "rgb(0, 0, 255)",
            backgroundColor: "rgba(0, 0, 255, 0.3)",
            // fill: {
            //     target: { value: 0 },
            // },
        },
    ],
};

scoreChart.PLUGINS = {
    legend: {
        labels: {
            generateLabels: (chart) => {
                return chart.data.datasets.map((dataset, index) => ({
                    text: dataset.label + (dataset.data.length ? ": " + dataset.data.slice(-1) : ""),
                    fillStyle: dataset.backgroundColor,
                    strokeStyle: dataset.borderColor,
                    hidden: !chart.isDatasetVisible(index),
                    datasetIndex: index,
                }));
            },
        },
    },
};

scoreChart.SCALES = {
    y: {
        suggestedMin: 45,
        suggestedMax: 55,
        title: {
            display: true,
            text: "Winrate",
        },
        afterDataLimits: function (axis) {
            let maxDiff = axis.max - 50;
            let minDiff = 50 - axis.min;
            if (maxDiff > minDiff) {
                axis.min = 50 - maxDiff;
            } else if (minDiff > maxDiff) {
                axis.max = 50 + minDiff;
            }
        },
    },
    y1: {
        position: "right",
        suggestedMin: -2,
        suggestedMax: 2,
        title: {
            display: true,
            text: "Score",
        },
        grid: {
            color: function (context) {
                if (context.tick.value == 0) {
                    return theme.theme == theme.TYPES.DARK ? "rgb(200, 200, 200)" : "rgb(0, 0, 0)";
                }
                return theme.theme == theme.TYPES.DARK ? "rgb(33, 37, 41)" : "rgb(255, 255, 255)";
            },
        },
        afterDataLimits: function (axis) {
            if (axis.max > axis.min * -1) {
                axis.min = axis.max * -1;
            } else if (axis.min * -1 > axis.max) {
                axis.max = axis.min * -1;
            }
        },
    },
};

scoreChart.CONFIG = {
    type: "line",
    data: scoreChart.DATA,
    options: {
        responsive: true,
        interaction: {
            intersect: false,
            mode: "index",
        },
        animation: {
            duration: 0,
        },
        plugins: scoreChart.PLUGINS,
        scales: scoreChart.SCALES,
    },
};


scoreChart.init = function () {
    scoreChart.colorElement = document.getElementById("scoreChartColor");

    scoreChart.element = document.getElementById("scoreChart");
    scoreChart.chart = new Chart(scoreChart.element, scoreChart.CONFIG);
    scoreChart.labels = scoreChart.chart.data.labels;
    scoreChart.winrates = scoreChart.chart.data.datasets[0].data;
    scoreChart.scores = scoreChart.chart.data.datasets[1].data;

    scoreChart.chart.canvas.onclick = scoreChart.canvasClickListener;
    scoreChart.colorElement.addEventListener("input", scoreChart.colorElementInputListener);

    theme.themeChangedEvent.add(scoreChart.themeChangedListener);

    scoreChart.clear();
};

scoreChart.clear = function () {
    scoreChart.clearChart();
    scoreChart.history = new History();

    if (trainerG.isLoadingServerData) {
        scoreChart.fillHistoryWithSuggestionHistory();
    }

    if (debug.testData == 1) {
        scoreChart.history.add(new Score(5_0_1_00000, 0), 1, 0);
        scoreChart.history.add(new Score(5_0_2_00000, 0), 2, 0);
        scoreChart.history.add(new Score(5_0_3_00000, 0), 3, 0);
        scoreChart.history.add(new Score(5_0_4_00000, 0), 4, 0);
        scoreChart.history.add(new Score(5_0_5_00000, 0), 5, 0);

        scoreChart.history.add(new Score(5_1_1_00000, 0), 1, 1);
        scoreChart.history.add(new Score(5_1_3_00000, 0), 3, 1);
        scoreChart.history.add(new Score(5_1_4_00000, 0), 4, 1);
        scoreChart.history.add(new Score(5_1_5_00000, 0), 5, 1);

        scoreChart.history.add(new Score(5_2_5_00000, 0), 5, 2);
        scoreChart.history.add(new Score(5_2_6_00000, 0), 6, 2);
    }
};


scoreChart.clearChart = function () {
    scoreChart.labels.length = 0;
    scoreChart.winrates.length = 0;
    scoreChart.scores.length = 0;
    scoreChart.chart.update();
};

scoreChart.fillHistoryWithSuggestionHistory = function (node = trainerG.board.editor.getRoot()) {
    for (let i = 0; i < node.children.length; i++) {
        scoreChart.fillHistoryWithSuggestionHistory(node.children[i]);
    }

    if (!node.move) return;

    let suggestionList = trainerG.suggestionsHistory.get(node.navTreeX, node.navTreeY);
    if (!suggestionList) return;

    let suggestion = suggestionList.find(new Coord(node.move.x, node.move.y));
    if (!suggestion) {
        suggestion = suggestionList.analyzeMoveSuggestion;
    }

    scoreChart.history.add(suggestion.score, node.navTreeX, node.navTreeY);
};

scoreChart.canvasClickListener = function (click) {
    const points = scoreChart.chart.getElementsAtEventForMode(click, "nearest", { intersect: false }, true);
    if (points[0]) {
        trainerG.board.goToNode(scoreChart.labels[points[0].index]);
    }
};

scoreChart.themeChangedListener = function () {
    scoreChart.chart.update();
};

scoreChart.update = function (suggestion) {
    let moveNumber = trainerG.board.getMoveNumber();
    if (scoreChart.labels.includes(moveNumber)) return;

    let index;
    for (index = 0; index < scoreChart.labels.length; index++) {
        if (scoreChart.labels[index] > moveNumber) {
            break;
        }
    }

    scoreChart.labels.splice(index, 0, moveNumber);
    
    let type = stats.PLAYER_RESULT_TYPE.WRONG;
    if (trainerG.isPerfectChoice) {
        type = stats.PLAYER_RESULT_TYPE.PERFECT;
    } else if (trainerG.isRightChoice) {
        type = stats.PLAYER_RESULT_TYPE.RIGHT;
    }

    stats.playerResultHistory.add(type);

    let point = suggestion.score.copy();
    scoreChart.history.add(point, moveNumber);

    let winrate = point.formatWinrate();
    scoreChart.winrates.splice(index, 0, winrate);

    let score = point.formatScoreLead();
    scoreChart.scores.splice(index, 0, score);

    scoreChart.chart.update();
};

scoreChart.refresh = function () {
    let points = [];
    let node = trainerG.board.get();
    do {
        let x = node.navTreeX;
        let y = node.navTreeY;

        node = node.parent;

        let point = scoreChart.history.get(x, y);
        if (!point) continue;

        point.index = x;
        points.push(point);
    } while (node);

    points = points.reverse();
    let i;
    for (i = 0; i < points.length; i++) {
        let point = points[i];
        scoreChart.labels[i] = point.index;
        scoreChart.winrates[i] = point.formatWinrate();
        scoreChart.scores[i] = point.formatScoreLead();
    }

    scoreChart.labels.length = i;
    scoreChart.winrates.length = i;
    scoreChart.scores.length = i;

    scoreChart.chart.update();
};

scoreChart.colorElementInputListener = function () {
    scoreChart.reverse();
};

scoreChart.reverse = function () {
    for (let i = 0; i < scoreChart.winrates.length; i++) {
        scoreChart.winrates[i] = (100 - scoreChart.winrates[i]).toFixed(2);
    }

    for (let i = 0; i < scoreChart.scores.length; i++) {
        scoreChart.scores[i] = (scoreChart.scores[i] * -1).toFixed(1);
    }

    scoreChart.chart.update();

    for (const point of scoreChart.history.iterateData()) {
        point.reverse();
    }
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.scoreChart) window.trainer.scoreChart = scoreChart;

export { scoreChart };
