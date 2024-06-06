import { stats } from "./stats";

let cUserChart = {};

cUserChart.init = function (dayLabels, monthLabels, dayUsers, dayActiveUsers, dayNewUsers, monthUsers, monthActiveUsers, monthNewUsers) {
    let dayUserChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    dayUserChartConfig.data.labels = dayLabels;
    dayUserChartConfig.data.datasets.push({
        label: 'Users',
        data: dayUsers,
        borderColor: 'rgb(200, 200, 200)',
    });
    dayUserChartConfig.data.datasets.push({
        label: 'Active users',
        data: dayActiveUsers,
        borderColor: 'rgb(0, 255, 0)',
    });
    dayUserChartConfig.data.datasets.push({
        label: 'New users',
        data: dayNewUsers,
        borderColor: 'rgb(0, 0, 255)',
    });
    dayUserChartConfig.options.scales.y = {
        ticks: {
            stepSize: 1,
        },
    };

    cUserChart.dayUserChartElement = document.getElementById("dayUserChart");
    cUserChart.dayUserChart = new Chart(cUserChart.dayUserChartElement, dayUserChartConfig);
    

    let monthUserChartConfig = utils.deepCopyObject(dayUserChartConfig);
    monthUserChartConfig.data.labels = monthLabels;
    monthUserChartConfig.data.datasets[0].data = monthUsers;
    monthUserChartConfig.data.datasets[1].data = monthActiveUsers;
    monthUserChartConfig.data.datasets[2].data = monthNewUsers;

    cUserChart.monthUserChartElement = document.getElementById("monthUserChart");
    cUserChart.monthUserChart = new Chart(cUserChart.monthUserChartElement, monthUserChartConfig);
};

export { cUserChart };
