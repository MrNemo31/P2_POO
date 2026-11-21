// Ajusta somente a navegação dos filtros, sem consultar ou gravar dados.
document.querySelectorAll("[data-resolution-filter]").forEach((form) => {
    form.addEventListener("submit", (event) => {
        event.preventDefault();
        const url = new URL(form.action);
        url.search = "";
        new FormData(form).forEach((value, key) => {
            if (value !== "") url.searchParams.append(key, value);
        });
        window.location.assign(url);
    });
});

// Avaliação e envio de respostas pertencem à integração com o back-end.
document.querySelectorAll("[data-resolution-form]").forEach((form) => {
    form.addEventListener("submit", (event) => event.preventDefault());
});
