using LiteArt.Poo.Models.ViewModels;

namespace LiteArt.Poo.Previa;

// Apenas rotas GET para abrir as Views vazias. Sem consultas, avaliação ou registro de respostas.
public static class ResolucaoQuestaoRotas
{
    public static void MapearResolucaoQuestaoPrevia(this WebApplication app)
    {
        app.MapGet("/Estudo", (int? alunoId, int? materiaId) => new PaginaRazor("Estudo", "Index", new EstudoViewModel
        {
            AlunoId = alunoId, MateriaId = materiaId
        })).WithName("EstudoIndex");
        app.MapGet("/Estudo/Resolver/{id:int}", (int id, int? alunoId, int? materiaId) => new PaginaRazor("Estudo", "Resolver", new ResolverViewModel
        {
            Id = id, AlunoId = alunoId, MateriaId = materiaId
        })).WithName("EstudoResolver");
    }
}
