namespace LiteArt.Poo.Models.ViewModels;

// Somente os dados de apresentação do formulário. Sem regras de negócio ou validação de servidor.
public class QuestaoFormularioViewModel
{
    public bool AguardandoDados { get; init; }
    public int? Id { get; set; }
    public string Enunciado { get; set; } = "";
    public int[] MateriaIds { get; set; } = [];
    public string Tipo { get; set; } = "MultiplaEscolha";
    public string?[] Alternativas { get; set; } = new string?[4];
    public int? AlternativaCorreta { get; set; }
    public bool? GabaritoVf { get; set; }
    public IReadOnlyList<MateriaOpcao> Materias { get; set; } = [];
}
