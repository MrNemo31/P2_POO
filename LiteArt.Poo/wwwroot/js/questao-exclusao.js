// Somente apresentação: impede envio até a integração com o back-end.
const confirmationForm = document.querySelector("[data-question-delete]");
confirmationForm?.addEventListener("submit", (event) => event.preventDefault());
