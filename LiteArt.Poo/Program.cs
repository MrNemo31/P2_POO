using LiteArt.Poo.Previa;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Somente rotas GET para visualizar as páginas. Sem ações de controllers.
app.MapearBancoDeQuestoesPrevia();
app.MapearResolucaoQuestaoPrevia();
app.MapearExclusaoQuestaoPrevia();
app.MapearHistoricoPrevia();
app.MapearFormularioQuestaoPrevia();
app.MapearPaginasDePrevia();

app.Run();
