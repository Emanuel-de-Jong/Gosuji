import { Ratio } from "../classes/Ratio";
import { stats } from "./stats";
import { trainerG } from "./trainerG";

let ratioChart = { id: "ratioChart" };


ratioChart.DATA = {
    datasets: [
        {
            label: "Right",
            pointRadius: 1,
            borderColor: "rgb(0, 255, 0)",
            backgroundColor: "rgba(0, 255, 0, 0.3)",
        },
        {
            label: "Perfect (A)",
            pointRadius: 1,
            borderColor: "rgb(0, 0, 255)",
            backgroundColor: "rgba(0, 0, 255, 0.3)",
        },
    ],
};

ratioChart.PLUGINS = {
    legend: {
        labels: {
            generateLabels: (chart) => {
                return chart.data.datasets.map((dataset, index) => ({
                    text: dataset.label + (dataset.data.length ? ": " + dataset.data.slice(-1) + "%" : ""),
                    fillStyle: dataset.backgroundColor,
                    strokeStyle: dataset.borderColor,
                    hidden: !chart.isDatasetVisible(index),
                    datasetIndex: index,
                }));
            },
        },
    },
};

ratioChart.SCALES = {
    y: {
        suggestedMin: 0,
        suggestedMax: 100,
        title: {
            display: true,
            text: "Percent",
        },
    },
};

ratioChart.CONFIG = {
    type: "line",
    data: ratioChart.DATA,
    options: {
        responsive: true,
        interaction: {
            intersect: false,
            mode: "index",
        },
        animation: {
            duration: 0,
        },
        plugins: ratioChart.PLUGINS,
        scales: ratioChart.SCALES,
    },
};


ratioChart.init = function () {
    ratioChart.element = document.getElementById("ratioChart");
    ratioChart.chart = new Chart(ratioChart.element, ratioChart.CONFIG);
    ratioChart.labels = ratioChart.chart.data.labels;
    ratioChart.rights = ratioChart.chart.data.datasets[0].data;
    ratioChart.perfects = ratioChart.chart.data.datasets[1].data;

    ratioChart.chart.canvas.onclick = ratioChart.canvasClickListener;

    theme.themeChangedEvent.add(ratioChart.themeChangedListener);

    ratioChart.clear();
    ratioChart.isInitialized = true;
};

ratioChart.clear = function () {
    ratioChart.clearChart();
};


ratioChart.clearChart = function () {
    ratioChart.labels.length = 0;
    ratioChart.rights.length = 0;
    ratioChart.perfects.length = 0;
    ratioChart.chart.update();
};

ratioChart.getRatio = function (rangeStart, rangeEnd = Number.MAX_SAFE_INTEGER, node = trainerG.board.get()) {
    if (rangeStart != null) {
        node = trainerG.board.editor.getRoot();
        while (node.children.length != 0 && node.moveNumber < rangeEnd) {
            if (node.children[0].moveNumber > rangeEnd) break;
            node = node.children[0];
        }
    }

    let moveNumber = node.moveNumber;

    let playerResults = [];
    while (node && (rangeStart == null || node.moveNumber >= rangeStart)) {
        node = node.parent;

        let playerResult = stats.playerResultHistory.get(node);
        if (!playerResult) continue;

        playerResults.push(playerResult);
    }

    if (playerResults.length == 0) {
        return null;
    }

    playerResults = playerResults.reverse();

    let perfect = 0;
    let right = 0;
    for (const playerResult of playerResults) {
        if (playerResult == stats.PLAYER_RESULT_TYPE.PERFECT || playerResult == stats.PLAYER_RESULT_TYPE.RIGHT) {
            right++;
        }

        if (playerResult == stats.PLAYER_RESULT_TYPE.PERFECT) {
            perfect++;
        }
    }

    return new Ratio(moveNumber, playerResults.length, right, perfect);
};

ratioChart.canvasClickListener = function (click) {
    const points = ratioChart.chart.getElementsAtEventForMode(click, "nearest", { intersect: false }, true);
    if (points[0]) {
        trainerG.board.goToNode(ratioChart.labels[points[0].index]);
    }
};

ratioChart.themeChangedListener = function () {
    ratioChart.chart.update();
};

ratioChart.update = function () {
    let moveNumber = trainerG.board.getMoveNumber();
    if (ratioChart.labels.includes(moveNumber)) return;

    let ratio = ratioChart.getRatio();
    if (ratio == null) {
        return;
    }

    let index;
    for (index = 0; index < ratioChart.labels.length; index++) {
        if (ratioChart.labels[index] > moveNumber) {
            break;
        }
    }

    ratioChart.labels.splice(index, 0, moveNumber);

    ratioChart.rights.splice(index, 0, ratio.getRightPercent());
    ratioChart.perfects.splice(index, 0, ratio.getPerfectPercent());

    ratioChart.chart.update();
};

ratioChart.refresh = function () {
    let ratios = [];
    let node = trainerG.board.get();
    do {
        const currentNode = node;
        node = node.parent;

        if (trainerG.moveOriginHistory.get(currentNode) !== trainerG.MOVE_ORIGIN.PLAYER) {
            continue;
        }

        const ratio = ratioChart.getRatio(null, null, currentNode);

        if (!ratio) continue;

        ratio.index = currentNode.navTreeX;
        ratios.push(ratio);
    } while (node);

    ratios = ratios.reverse();
    let i;
    for (i = 0; i < ratios.length; i++) {
        let ratio = ratios[i];
        ratioChart.labels[i] = ratio.index;
        ratioChart.rights[i] = ratio.getRightPercent();
        ratioChart.perfects[i] = ratio.getPerfectPercent();
    }

    ratioChart.labels.length = i;
    ratioChart.rights.length = i;
    ratioChart.perfects.length = i;

    ratioChart.chart.update();
};

if (!window.trainer) window.trainer = {};
if (g.DEBUG && !window.trainer.ratioChart) window.trainer.ratioChart = ratioChart;

export { ratioChart };
