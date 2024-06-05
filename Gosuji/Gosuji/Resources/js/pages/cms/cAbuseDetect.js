let cAbuseDetect = {};

cAbuseDetect.init = function () {
    this.table = new DataTable("#abuseTable", {
        order: [[2, "desc"]]
    });
};

export { cAbuseDetect };
