using LiteArt.Poo.Models.ViewModels;

namespace LiteArt.Poo.Previa;

// Rotas GET apenas para abrir as Views. Não carregam, validam ou salvam questões.
public static class FormularioQuestaoRotas
{
    public static void MapearFormularioQuestaoPrevia(this WebApplication app)
    {
        app.MapGet("/Questoes/Nova", () => new PaginaRazor("Questoes", "Nova", new QuestaoFormularioViewModel()))
            .WithName("QuestoesNova");
        app.MapGet("/Questoes/Editar/{id:int}", (int id) => new PaginaRazor("Questoes", "Editar", new QuestaoFormularioViewModel
        {
            Id = id, AguardandoDados = true
        })).WithName("QuestoesEditar");
    }
}
