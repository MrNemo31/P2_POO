namespace LiteArt.Poo.Previa;

// Rotas de apresentação sem dados de exemplo, consultas ou operações de negócio.
public static class PreviaRotas
{
    public static void MapearPaginasDePrevia(this WebApplication app)
    {
        app.MapGet("/", () => new PaginaRazor("Home", "Index"));
        app.MapGet("/Home/Index", () => new PaginaRazor("Home", "Index"));
    }
}
