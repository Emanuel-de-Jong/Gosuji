import { statsPage } from "./stats";

let cUserChart = {};

cUserChart.init = function (dayLabels, monthLabels, dayUsers, dayActiveUsers, dayNewUsers, monthUsers, monthActiveUsers, monthNewUsers) {
    let dayChartConfig = utils.deepCopyObject(statsPage.BASE_CONFIG);
    dayChartConfig.data.labels = dayLabels;
    dayChartConfig.data.datasets.push({
        label: 'Users',
        data: dayUsers,
        borderColor: 'rgb(200, 200, 200)',
    });
    dayChartConfig.data.datasets.push({
        label: 'Active users',
        data: dayActiveUsers,
        borderColor: 'rgb(0, 255, 0)',
    });
    dayChartConfig.data.datasets.push({
        label: 'New users',
        data: dayNewUsers,
        borderColor: 'rgb(0, 0, 255)',
    });
    dayChartConfig.options.scales.y = {
        ticks: {
            stepSize: 1,
        },
    };

    cUserChart.dayChartElement = document.getElementById("dayUserChart");
    cUserChart.dayChart = new Chart(cUserChart.dayChartElement, dayChartConfig);
    

    let monthChartConfig = utils.deepCopyObject(dayChartConfig);
    monthChartConfig.data.labels = monthLabels;
    monthChartConfig.data.datasets[0].data = monthUsers;
    monthChartConfig.data.datasets[1].data = monthActiveUsers;
    monthChartConfig.data.datasets[2].data = monthNewUsers;

    cUserChart.monthChartElement = document.getElementById("monthUserChart");
    cUserChart.monthChart = new Chart(cUserChart.monthChartElement, monthChartConfig);
};

export { cUserChart };
