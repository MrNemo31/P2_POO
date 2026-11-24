namespace LiteArt.Poo.Models.ViewModels;

// Modelos exclusivos da apresentação do histórico.
public record TentativaPreviaViewModel(QuestaoPreviaViewModel Questao, bool Acertou, string Quando)
{
    public int Id { get; init; }
}

public class HistoricoViewModel
{
    public IReadOnlyList<AlunoOpcao> Alunos { get; init; } = [];
    public int? AlunoId { get; init; }
    public IReadOnlyList<TentativaPreviaViewModel> Tentativas { get; init; } = [];
}
