document.querySelectorAll("[data-question-form]").forEach((form) => {
    // Somente apresentação: alterna os campos e impede envio sem integração.
    form.addEventListener("submit", (event) => event.preventDefault());
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
