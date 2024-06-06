let stats = {};

// https://www.chartjs.org/docs/3.9.1/
stats.BASE_DATA = {
    datasets: [
        
    ],
};

stats.BASE_PLUGINS = {
};

stats.BASE_SCALES = {
};

stats.BASE_CONFIG = {
    type: "line",
    data: stats.BASE_DATA,
    options: {
        responsive: true,
        interaction: {
            intersect: false,
            mode: "index",
        },
        animation: {
            duration: 0,
        },
        plugins: stats.BASE_PLUGINS,
        scales: stats.BASE_SCALES,
    },
};


stats.init = function (dayLabels, monthLabels, dayUsers, dayActiveUsers, dayNewUsers, monthUsers, monthActiveUsers, monthNewUsers) {
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

    stats.dayUserChartElement = document.getElementById("dayUserChart");
    stats.dayUserChart = new Chart(stats.dayUserChartElement, dayUserChartConfig);
    

    let monthUserChartConfig = utils.deepCopyObject(dayUserChartConfig);
    monthUserChartConfig.data.labels = monthLabels;
    monthUserChartConfig.data.datasets[0].data = monthUsers;
    monthUserChartConfig.data.datasets[1].data = monthActiveUsers;
    monthUserChartConfig.data.datasets[2].data = monthNewUsers;

    stats.monthUserChartElement = document.getElementById("monthUserChart");
    stats.monthUserChart = new Chart(stats.monthUserChartElement, monthUserChartConfig);
};

export { stats };
