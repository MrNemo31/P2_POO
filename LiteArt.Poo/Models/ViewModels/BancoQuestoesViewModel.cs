namespace LiteArt.Poo.Models.ViewModels;

// Modelos de apresentação do banco e dos seus itens.
public record MateriaOpcao(int Id, string Nome);

public class QuestaoPreviaViewModel
{
    public int Id { get; init; }
    public string Enunciado { get; init; } = "";
    public IReadOnlyList<MateriaOpcao> Materias { get; init; } = [];
    public string Tipo { get; init; } = "MultiplaEscolha";
    public string TipoTexto => Tipo == "MultiplaEscolha" ? "Múltipla escolha" : "Verdadeiro/falso";
    public IReadOnlyList<string> Alternativas { get; init; } = [];
    public int AlternativaCorreta { get; init; }
    public bool GabaritoVf { get; init; }
    public bool TemTentativas { get; init; }
    public string Trecho => Enunciado.Length > 140 ? Enunciado[..140] + "…" : Enunciado;
}

public class BancoQuestoesViewModel
{
    public int? MateriaId { get; init; }
    public IReadOnlyList<MateriaOpcao> Materias { get; init; } = [];
    public IReadOnlyList<QuestaoPreviaViewModel> Questoes { get; init; } = [];
}
