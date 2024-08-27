import { History } from "../classes/History";
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
};

ratioChart.clear = function () {
    ratioChart.clearChart();
    ratioChart.history = new History();
};


ratioChart.clearChart = function () {
    ratioChart.labels.length = 0;
    ratioChart.rights.length = 0;
    ratioChart.perfects.length = 0;
    ratioChart.chart.update();
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

    let index;
    for (index = 0; index < ratioChart.labels.length; index++) {
        if (ratioChart.labels[index] > moveNumber) {
            break;
        }
    }

    ratioChart.labels.splice(index, 0, moveNumber);

    let point = suggestion.score.copy();
    ratioChart.history.add(point, moveNumber);

    let winrate = point.formatWinrate();
    ratioChart.rights.splice(index, 0, winrate);

    let score = point.formatScoreLead();
    ratioChart.perfects.splice(index, 0, score);

    ratioChart.chart.update();
};

ratioChart.refresh = function () {
    let points = [];
    let node = trainerG.board.editor.getCurrent();
    do {
        let x = node.navTreeX;
        let y = node.navTreeY;

        node = node.parent;

        let point = ratioChart.history.get(x, y);
        if (!point) continue;

        point.index = x;
        points.push(point);
    } while (node);

    points = points.reverse();
    let i;
    for (i = 0; i < points.length; i++) {
        let point = points[i];
        ratioChart.labels[i] = point.index;
        ratioChart.rights[i] = point.formatWinrate();
        ratioChart.perfects[i] = point.formatScoreLead();
    }

    ratioChart.labels.length = i;
    ratioChart.rights.length = i;
    ratioChart.perfects.length = i;

    ratioChart.chart.update();
};

export { ratioChart };
