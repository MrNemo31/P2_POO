// Somente navegação do filtro. A consulta de tentativas pertence ao back-end.
document.querySelectorAll("[data-history-filter]").forEach((form) => {
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
