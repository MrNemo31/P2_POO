document.querySelectorAll("[data-question-form]").forEach((form) => {
    // Alterna os campos; a validação e a gravação são feitas pelo controller.
    const typeSelect = form.querySelector("[data-question-type]");

    function updateAnswerFields() {
        form.querySelectorAll("[data-type-fields]").forEach((fieldset) => {
            const active = fieldset.dataset.typeFields === typeSelect.value;
            fieldset.hidden = !active;
            fieldset.disabled = !active;
        });
    }

    typeSelect.addEventListener("change", updateAnswerFields);
    updateAnswerFields();
});
