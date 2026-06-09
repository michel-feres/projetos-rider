const temaSalvo = localStorage.getItem("biblioteca-tema") || "claro";
document.documentElement.dataset.theme = temaSalvo;

document.addEventListener("DOMContentLoaded", () => {
    const botaoTema = document.getElementById("themeToggle");
    if (!botaoTema) {
        return;
    }

    const atualizarTexto = () => {
        botaoTema.textContent = document.documentElement.dataset.theme === "escuro" ? "Tema claro" : "Tema escuro";
    };

    botaoTema.addEventListener("click", () => {
        const proximoTema = document.documentElement.dataset.theme === "escuro" ? "claro" : "escuro";
        document.documentElement.dataset.theme = proximoTema;
        localStorage.setItem("biblioteca-tema", proximoTema);
        atualizarTexto();
    });

    atualizarTexto();
});
