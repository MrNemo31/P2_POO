namespace LiteArt.Poo.Previa;

// Abre somente a View de exclusão, sem carregar ou excluir dados.
public static class ExclusaoQuestaoRotas
{
    public static void MapearExclusaoQuestaoPrevia(this WebApplication app)
    {
        app.MapGet("/Questoes/Excluir/{id:int}", (int id) => new PaginaRazor("Questoes", "Excluir"))
            .WithName("QuestoesExcluir");
    }
}
