using LiteArt.Poo.Models.ViewModels;

namespace LiteArt.Poo.Previa;

// Abre a View vazia. Não consulta alunos nem tentativas e não altera dados.
public static class HistoricoRotas
{
    public static void MapearHistoricoPrevia(this WebApplication app)
    {
        app.MapGet("/Historico", (int? alunoId) => new PaginaRazor("Historico", "Index", new HistoricoViewModel
        {
            AlunoId = alunoId
        })).WithName("HistoricoIndex");
    }
}
