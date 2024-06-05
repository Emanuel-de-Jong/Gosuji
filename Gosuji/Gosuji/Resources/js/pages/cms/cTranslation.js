let cTranslation = {};

cTranslation.resizeTranslationTextareas = function () {
    let textareas = document.querySelectorAll("#translationInputs textarea");
    textareas.forEach(t => t.dispatchEvent(new Event("input")));
};

export { cTranslation };
