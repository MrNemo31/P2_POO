document.documentElement.classList.add("js");

const menuButton = document.querySelector(".menu-toggle");
const navigation = document.querySelector("#navegacao");

menuButton?.addEventListener("click", () => {
    const expanded = menuButton.getAttribute("aria-expanded") === "true";
    menuButton.setAttribute("aria-expanded", String(!expanded));
    navigation?.classList.toggle("is-open", !expanded);
});

navigation?.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
        navigation.classList.remove("is-open");
        menuButton?.setAttribute("aria-expanded", "false");
        menuButton?.focus();
    }
});
