import { History } from "../classes/History";
import { trainerG } from "./trainerG";

let skillChart = { id: "skillChart" };


skillChart.DATA = {
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

skillChart.PLUGINS = {
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

skillChart.SCALES = {
    y: {
        suggestedMin: 0,
        suggestedMax: 100,
        title: {
            display: true,
            text: "Percent",
        },
    },
};

skillChart.CONFIG = {
    type: "line",
    data: skillChart.DATA,
    options: {
        responsive: true,
        interaction: {
            intersect: false,
            mode: "index",
        },
        animation: {
            duration: 0,
        },
        plugins: skillChart.PLUGINS,
        scales: skillChart.SCALES,
    },
};


skillChart.init = function (gameLoadInfo) {
    skillChart.element = document.getElementById("skillChart");
    skillChart.chart = new Chart(skillChart.element, skillChart.CONFIG);
    skillChart.labels = skillChart.chart.data.labels;
    skillChart.rights = skillChart.chart.data.datasets[0].data;
    skillChart.perfects = skillChart.chart.data.datasets[1].data;

    skillChart.chart.canvas.onclick = skillChart.canvasClickListener;

    theme.themeChangedEvent.add(skillChart.themeChangedListener);

    skillChart.clear(gameLoadInfo);
};

skillChart.clear = function (gameLoadInfo) {
    skillChart.clearChart();
    skillChart.history = gameLoadInfo ? History.fromServer(gameLoadInfo.playerResults) : new History();
};


skillChart.clearChart = function () {
    skillChart.labels.length = 0;
    skillChart.rights.length = 0;
    skillChart.perfects.length = 0;
    skillChart.chart.update();
};

skillChart.canvasClickListener = function (click) {
    const points = skillChart.chart.getElementsAtEventForMode(click, "nearest", { intersect: false }, true);
    if (points[0]) {
        trainerG.board.goToNode(skillChart.labels[points[0].index]);
    }
};

skillChart.themeChangedListener = function () {
    skillChart.chart.update();
};

skillChart.update = function () {
    let moveNumber = trainerG.board.getMoveNumber();
    if (skillChart.labels.includes(moveNumber)) return;

    let index;
    for (index = 0; index < skillChart.labels.length; index++) {
        if (skillChart.labels[index] > moveNumber) {
            break;
        }
    }

    skillChart.labels.splice(index, 0, moveNumber);

    let point = suggestion.score.copy();
    skillChart.history.add(point, moveNumber);

    let winrate = point.formatWinrate();
    skillChart.rights.splice(index, 0, winrate);

    let score = point.formatScoreLead();
    skillChart.perfects.splice(index, 0, score);

    skillChart.chart.update();
};

skillChart.refresh = function () {
    let points = [];
    let node = trainerG.board.editor.getCurrent();
    do {
        let x = node.navTreeX;
        let y = node.navTreeY;

        node = node.parent;

        let point = skillChart.history.get(x, y);
        if (!point) continue;

        point.index = x;
        points.push(point);
    } while (node);

    points = points.reverse();
    let i;
    for (i = 0; i < points.length; i++) {
        let point = points[i];
        skillChart.labels[i] = point.index;
        skillChart.rights[i] = point.formatWinrate();
        skillChart.perfects[i] = point.formatScoreLead();
    }

    skillChart.labels.length = i;
    skillChart.rights.length = i;
    skillChart.perfects.length = i;

    skillChart.chart.update();
};

export { skillChart };
