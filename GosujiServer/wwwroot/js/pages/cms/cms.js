var cms = {};


cms.resizeTranslationTextareas = function () {
    let textareas = document.querySelectorAll("#translationInputs textarea");
    textareas.forEach(t => t.dispatchEvent(new Event("input")));
};
