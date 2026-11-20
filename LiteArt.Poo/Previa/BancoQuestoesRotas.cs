using LiteArt.Poo.Models.ViewModels;

namespace LiteArt.Poo.Previa;

// Rota GET de apresentação. Os dados serão fornecidos pela integração com o back-end.
public static class BancoQuestoesRotas
{
    public static void MapearBancoDeQuestoesPrevia(this WebApplication app)
    {
        app.MapGet("/Questoes", (int? materiaId) => new PaginaRazor("Questoes", "Index", new BancoQuestoesViewModel
        {
            MateriaId = materiaId
        })).WithName("QuestoesIndex");
    }
}
