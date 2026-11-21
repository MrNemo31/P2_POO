using LiteArt.Poo.Previa;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Rota de apresentação do banco de questões; não altera dados.
app.MapearBancoDeQuestoesPrevia();
app.MapearExclusaoQuestaoPrevia();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
