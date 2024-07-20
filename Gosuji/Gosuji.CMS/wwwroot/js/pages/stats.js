var statsPage = {};

// https://www.chartjs.org/docs/3.9.1/
statsPage.BASE_DATA = {
    datasets: [
        
    ],
};

statsPage.BASE_PLUGINS = {
};

statsPage.BASE_SCALES = {
};

statsPage.BASE_CONFIG = {
    type: "line",
    data: statsPage.BASE_DATA,
    options: {
        responsive: true,
        interaction: {
            intersect: false,
            mode: "index",
        },
        animation: {
            duration: 0,
        },
        plugins: statsPage.BASE_PLUGINS,
        scales: statsPage.BASE_SCALES,
    },
};
