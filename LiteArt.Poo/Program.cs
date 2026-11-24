using LiteArt.Poo.Previa;
using LiteArt.Poo.Data;
using LiteArt.Poo.Repositories.Interfaces;
using LiteArt.Poo.Repositories.Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registra a conexão com o banco de dados.
builder.Services.AddSingleton<DatabaseConnection>();

//cria uma instância do repositório para cada requisição HTTP e reutiliza essa instância durante aquela requisição.
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();

builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();

builder.Services.AddScoped<IQuestaoRepository, QuestaoRepository>();

builder.Services.AddScoped<IAdministradorConsulta, AdministradorConsulta>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Rota de apresentação do banco de questões; não altera dados.
app.MapearBancoDeQuestoesPrevia();
app.MapearResolucaoQuestaoPrevia();
app.MapearExclusaoQuestaoPrevia();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();