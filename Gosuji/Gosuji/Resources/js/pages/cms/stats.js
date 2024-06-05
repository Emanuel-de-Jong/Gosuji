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


stats.init = function (dayLabels, monthLabels, dayUsers, dayNewUsers, monthUsers, monthNewUsers) {
    let dayUserChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    dayUserChartConfig.data.labels = dayLabels;
    dayUserChartConfig.data.datasets.push({
        label: 'Users',
        data: dayUsers,
        borderColor: 'rgb(75, 192, 192)',
    });
    dayUserChartConfig.data.datasets.push({
        label: 'New users',
        data: dayNewUsers,
        borderColor: 'rgb(200, 192, 50)',
    });

    stats.dayUserChartElement = document.getElementById("dayUserChart");
    stats.dayUserChart = new Chart(stats.dayUserChartElement, dayUserChartConfig);
    

    let monthUserChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    monthUserChartConfig.data.labels = monthLabels;
    monthUserChartConfig.data.datasets.push({
        label: 'Users',
        data: monthUsers,
        borderColor: 'rgb(75, 192, 192)',
    });
    monthUserChartConfig.data.datasets.push({
        label: 'New users',
        data: monthNewUsers,
        borderColor: 'rgb(200, 192, 50)',
    });

    stats.monthUserChartElement = document.getElementById("monthUserChart");
    stats.monthUserChart = new Chart(stats.monthUserChartElement, monthUserChartConfig);
};

export { stats };
