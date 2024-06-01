let abuseDetect = {};

abuseDetect.init = function () {
    this.table = new DataTable("#abuseTable", {
        order: [[2, "desc"]]
    });
};

export { abuseDetect };
