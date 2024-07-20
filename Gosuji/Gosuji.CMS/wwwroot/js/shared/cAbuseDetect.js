var cAbuseDetect = {};

cAbuseDetect.init = function () {
    this.abuseTable = new DataTable("#abuseTable", {
        order: [[2, "desc"]]
    });

    this.rateLimitTable = new DataTable("#rateLimitTable", {
        order: [[1, "desc"]]
    });
};
