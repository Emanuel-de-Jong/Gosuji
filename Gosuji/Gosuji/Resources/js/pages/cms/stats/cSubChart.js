import { stats } from "./stats";

let cSubChart = {};

cSubChart.init = function (dayLabels, monthLabels, daySubs, dayNewSubs, monthSubs, monthNewSubs) {
    let dayChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    dayChartConfig.data.labels = dayLabels;
    dayChartConfig.data.datasets.push({
        label: 'Subscriptions',
        data: daySubs,
        borderColor: 'rgb(200, 200, 200)',
    });
    dayChartConfig.data.datasets.push({
        label: 'New subscriptions',
        data: dayNewSubs,
        borderColor: 'rgb(0, 0, 255)',
    });
    dayChartConfig.options.scales.y = {
        ticks: {
            stepSize: 1,
        },
    };

    cSubChart.dayChartElement = document.getElementById("daySubChart");
    cSubChart.dayChart = new Chart(cSubChart.dayChartElement, dayChartConfig);
    

    let monthChartConfig = utils.deepCopyObject(dayChartConfig);
    monthChartConfig.data.labels = monthLabels;
    monthChartConfig.data.datasets[0].data = monthSubs;
    monthChartConfig.data.datasets[1].data = monthNewSubs;

    cSubChart.monthChartElement = document.getElementById("monthSubChart");
    cSubChart.monthChart = new Chart(cSubChart.monthChartElement, monthChartConfig);
};

export { cSubChart };
