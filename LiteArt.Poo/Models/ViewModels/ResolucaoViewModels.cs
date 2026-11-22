namespace LiteArt.Poo.Models.ViewModels;

// Somente dados de apresentação, sem validações de servidor ou regras de negócio.
public record AlunoOpcao(int Id, string Nome);

public class ResolverViewModel
{
    public int Id { get; set; }
    public int? AlunoId { get; set; }
    public int? MateriaId { get; set; }
    public string? Resposta { get; set; }
    // Resultado fornecido pelo back-end após o envio. Nulo mantém o feedback oculto.
    public bool? Acertou { get; set; }
    public QuestaoPreviaViewModel? Questao { get; set; }
    public IReadOnlyList<AlunoOpcao> Alunos { get; set; } = [];
}

public class EstudoViewModel
{
    public IReadOnlyList<AlunoOpcao> Alunos { get; init; } = [];
    public IReadOnlyList<MateriaOpcao> Materias { get; init; } = [];
    public int? AlunoId { get; init; }
    public int? MateriaId { get; init; }
    public IReadOnlyList<QuestaoPreviaViewModel> Questoes { get; init; } = [];
}
