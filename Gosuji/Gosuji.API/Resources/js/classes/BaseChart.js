let BaseChart = window.BaseChart;

if (typeof BaseChart === "undefined") {
    BaseChart = class BaseChart {
        CONFIG = {
            data: {},
            options: {
                responsive: true,
                interaction: {
                    intersect: false,
                    mode: "index",
                },
                animation: {
                    duration: 0,
                },
            },
        };


        init(element, type, datasets, scales, plugins) {
            this.element = element;
            this.CONFIG.type = type;
            this.CONFIG.data.datasets = datasets;
            this.CONFIG.options.scales = scales;
            this.CONFIG.options.plugins = plugins;

            this.chart = new Chart(this.element, this.CONFIG);

            theme.themeChangedEvent.add(this.chart.update);
        };

        
        clearData() {
            this.chart.data.labels.length = 0;
            for (let i=0; i<this.chart.data.datasets.length; i++) {
                this.chart.data.datasets[i].data.length = 0;
            }
            this.chart.update();
        };
    }

    window.BaseChart = BaseChart;
}

export { BaseChart };
