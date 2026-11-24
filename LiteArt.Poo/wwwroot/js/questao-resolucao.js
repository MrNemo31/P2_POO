// Mantém aluno e matéria na navegação dos filtros.
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

// O controller avalia e registra a resposta. Evita cliques repetidos no envio.
document.querySelectorAll("[data-resolution-form]").forEach((form) => {
    form.addEventListener("submit", () => {
        const button = form.querySelector("button[type=submit]");
        if (button) button.disabled = true;
    });
});
