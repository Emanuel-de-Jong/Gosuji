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

export { stats };
