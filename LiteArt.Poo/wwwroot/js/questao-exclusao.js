// A confirmação é enviada ao controller; evita cliques repetidos no botão.
const confirmationForm = document.querySelector("[data-question-delete]");
confirmationForm?.addEventListener("submit", () => {
    const button = confirmationForm.querySelector("button[type=submit]");
    if (button) button.disabled = true;
});
