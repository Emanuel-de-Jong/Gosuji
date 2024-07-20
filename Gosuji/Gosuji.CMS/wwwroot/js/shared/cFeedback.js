var cFeedback = {};

cFeedback.init = function (cFeedbackRef) {
    this.cFeedbackRef = cFeedbackRef;

    DataTable.ext.order["moment-pre"] = function(d) {
        return moment(d, "dd-MM-yy HH:mm", true).unix();
    };

    this.table = new DataTable("#feedbackTable", {
        order: [[4, "desc"]],
        columnDefs: [
            {
                targets: 4,
                type: "moment"
            }
        ],
        select: true,
    });

    this.table.on("select", cFeedback.selectListener);
};

cFeedback.selectListener = (e, dt, type, indexes) => {
    if (type !== "row") {
        return;
    }

    let feedbackId = parseInt(cFeedback.table.rows(indexes).nodes()[0].id);
    cFeedback.cFeedbackRef.invokeMethodAsync("SetSelectedFeedback", feedbackId);
};
