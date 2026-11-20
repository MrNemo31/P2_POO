// Omite seleções vazias na URL ao aplicar ou remover os filtros.
document.querySelectorAll(".filter-form").forEach((form) => {
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
