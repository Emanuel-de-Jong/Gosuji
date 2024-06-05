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


stats.init = function (dayLabels, monthLabels, users, newUsers) {
    let userChartConfig = utils.deepCopyObject(stats.BASE_CONFIG);
    userChartConfig.data.labels = dayLabels;
    userChartConfig.data.datasets.push({
        label: 'Users',
        data: users,
        borderColor: 'rgb(75, 192, 192)',
    });
    userChartConfig.data.datasets.push({
        label: 'New users',
        data: newUsers,
        borderColor: 'rgb(200, 192, 50)',
    });

    stats.userChartElement = document.getElementById("userChart");
    stats.userChart = new Chart(stats.userChartElement, userChartConfig);
};

export { stats };
