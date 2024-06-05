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


stats.init = function () {
    let userChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    userChartConfig.data.labels = [ 1, 2, 3 ];
    userChartConfig.data.datasets.push({
        label: 'Users',
        data: [65, 59, 80],
        borderColor: 'rgb(75, 192, 192)',
    });
    userChartConfig.data.datasets.push({
        label: 'New users',
        data: [20, 59, 40],
        borderColor: 'rgb(200, 192, 50)',
    });

    stats.userChartElement = document.getElementById("userChart");
    stats.userChart = new Chart(stats.userChartElement, userChartConfig);
};

export { stats };
