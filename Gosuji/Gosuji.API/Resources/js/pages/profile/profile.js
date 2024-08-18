let profilePage = {};

profilePage.DAYS_CHART_DAY_TYPES = {
    NONE: 0,
    CAUGHT_UP: 1,
    PLAYED: 2,
};

profilePage.DARK_THEME_GRID_COLOR = "rgb(100, 100, 100)";
profilePage.LIGHT_THEME_GRID_COLOR = "rgb(225, 225, 225)";


profilePage.init = function (rightColor, perfectColor) {
    profilePage.rightColor = rightColor;
    profilePage.perfectColor = perfectColor;

    theme.themeChangedEvent.add(profilePage.themeChangedListener);
};


profilePage.themeChangedListener = function (e) {
    let percentPerGameLineChartScales = profilePage.percentPerGameLineChart.config.options.scales;
    let gameStageBarChartScales = profilePage.gameStageBarChart.config.options.scales;
    if (e.theme == theme.TYPES.DARK) {
        percentPerGameLineChartScales.x.grid.color = profilePage.DARK_THEME_GRID_COLOR;
        percentPerGameLineChartScales.y.grid.color = profilePage.DARK_THEME_GRID_COLOR;
        gameStageBarChartScales.x.grid.color = profilePage.DARK_THEME_GRID_COLOR;
        gameStageBarChartScales.y.grid.color = profilePage.DARK_THEME_GRID_COLOR;
    } else {
        percentPerGameLineChartScales.x.grid.color = profilePage.LIGHT_THEME_GRID_COLOR;
        percentPerGameLineChartScales.y.grid.color = profilePage.LIGHT_THEME_GRID_COLOR;
        gameStageBarChartScales.x.grid.color = profilePage.LIGHT_THEME_GRID_COLOR;
        gameStageBarChartScales.y.grid.color = profilePage.LIGHT_THEME_GRID_COLOR;
    }

    profilePage.percentPerGameLineChart.update();
    profilePage.gameStageBarChartScales.update();
};

profilePage.createGameTable = function () {
    profilePage.gameTable = new DataTable("#gameTable", {
        order: [[13, "desc"]]
    });
};

profilePage.createPercentPerGameLineChart = function (rightPercents, perfectPercents) {
    let datasets = [];
    datasets.push({
        label: "Right",
        data: rightPercents,
        borderColor: profilePage.rightColor,
        backgroundColor: profilePage.rightColor,
    });
    datasets.push({
        label: "Perfect",
        data: perfectPercents,
        borderColor: profilePage.perfectColor,
        backgroundColor: profilePage.perfectColor,
    });

    let labels = [];
    for (let i = 1; i <= rightPercents.length; i++) {
        labels.push(i);
    }

    profilePage.percentPerGameLineChartCanvas = document.getElementById("percentPerGameLineChart");
    profilePage.percentPerGameLineChart = new Chart(profilePage.percentPerGameLineChartCanvas, {
        type: "line",
        data: {
            labels: labels,
            datasets: datasets,
        },
        options: {
            responsive: true,
            showLine: false,
            scales: {
                x: {
                    grid: {
                        color:
                        theme.theme == theme.TYPES.DARK
                                ? profilePage.DARK_THEME_GRID_COLOR
                                : profilePage.LIGHT_THEME_GRID_COLOR,
                    },
                },
                y: {
                    beginAtZero: true,
                    max: 100,
                    grid: {
                        color:
                        theme.theme == theme.TYPES.DARK
                                ? profilePage.DARK_THEME_GRID_COLOR
                                : profilePage.LIGHT_THEME_GRID_COLOR,
                    },
                },
            },
            interaction: {
                intersect: false,
                mode: "index",
            },
            animation: {
                duration: 0,
            },
        },
    });
};

profilePage.createGameStageBarChart = function (right, perfect) {
    let datasets = [];
    datasets.push({
        label: "Right",
        data: [right[0], right[1], right[2]],
        borderColor: profilePage.rightColor,
        backgroundColor: profilePage.rightColor,
    });
    datasets.push({
        label: "Perfect",
        data: [perfect[0], perfect[1], perfect[2]],
        borderColor: profilePage.perfectColor,
        backgroundColor: profilePage.perfectColor,
    });

    profilePage.gameStageBarChartCanvas = document.getElementById("gameStageBarChart");
    profilePage.gameStageBarChart = new Chart(profilePage.gameStageBarChartCanvas, {
        type: "bar",
        data: {
            labels: ["Opening", "Midgame", "Endgame"],
            datasets: datasets,
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    grid: {
                        color:
                            theme.theme == theme.TYPES.DARK
                                ? profilePage.DARK_THEME_GRID_COLOR
                                : profilePage.LIGHT_THEME_GRID_COLOR,
                    },
                },
                y: {
                    beginAtZero: true,
                    max: 100,
                    grid: {
                        color:
                            theme.theme == theme.TYPES.DARK
                                ? profilePage.DARK_THEME_GRID_COLOR
                                : profilePage.LIGHT_THEME_GRID_COLOR,
                    },
                },
            },
            interaction: {
                intersect: false,
                mode: "index",
            },
            animation: {
                duration: 0,
            },
        },
    });
};

profilePage.createDaysChart = function (days) {
    profilePage.daysChartElement = document.getElementById("daysChart");

    for (const date in days) {
        let dayElement = document.createElement("div");
        dayElement.classList.add("day");
        dayElement.setAttribute("title", date);

        let dayType = days[date];
        if (dayType != profilePage.DAYS_CHART_DAY_TYPES.NONE) {
            let color =
                dayType == profilePage.DAYS_CHART_DAY_TYPES.CAUGHT_UP
                    ? profilePage.rightColor
                    : profilePage.perfectColor;
            dayElement.style.backgroundColor = color;
        }

        profilePage.daysChartElement.appendChild(dayElement);
    }
};

export { profilePage };
