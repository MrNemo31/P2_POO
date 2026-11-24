using System.ComponentModel.DataAnnotations;

namespace LiteArt.Poo.Models.ViewModels;

public class QuestaoFormularioViewModel : IValidatableObject
{
    public bool AguardandoDados { get; init; }
    public int? Id { get; set; }
    [Required(ErrorMessage = "Informe o enunciado.")]
    [StringLength(2000, ErrorMessage = "O enunciado deve ter no máximo 2.000 caracteres.")]
    public string Enunciado { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de administrador maior que zero.")]
    public int IdAdministrador { get; set; }
    public int[] MateriaIds { get; set; } = [];
    public string Tipo { get; set; } = "MultiplaEscolha";
    public string?[] Alternativas { get; set; } = new string?[4];
    public int[] AlternativaIds { get; set; } = new int[4];
    public int? AlternativaCorreta { get; set; }
    public bool? GabaritoVf { get; set; }
    public IReadOnlyList<MateriaOpcao> Materias { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MateriaIds == null || MateriaIds.Length == 0 || MateriaIds.Any(id => id <= 0))
        {
            yield return new ValidationResult("Selecione uma ou mais matérias.", [nameof(MateriaIds)]);
        }

        if (Tipo == "MultiplaEscolha")
        {
            var textos = Alternativas ?? [];
            if (textos.Count(t => !string.IsNullOrWhiteSpace(t)) < 2)
            {
                yield return new ValidationResult("Preencha pelo menos duas alternativas.", [nameof(Alternativas)]);
            }
            if (textos.Any(t => t?.Length > 500))
            {
                yield return new ValidationResult("Cada alternativa deve ter no máximo 500 caracteres.", [nameof(Alternativas)]);
            }
            if (!AlternativaCorreta.HasValue || AlternativaCorreta < 0 || AlternativaCorreta >= textos.Length ||
                string.IsNullOrWhiteSpace(textos[AlternativaCorreta.Value]))
            {
                yield return new ValidationResult("Marque uma alternativa preenchida como correta.", [nameof(AlternativaCorreta)]);
            }
            if (AlternativaIds == null || AlternativaIds.Length != textos.Length ||
                AlternativaIds.Any(id => id < 0) ||
                AlternativaIds.Where(id => id > 0).Distinct().Count() != AlternativaIds.Count(id => id > 0))
            {
                yield return new ValidationResult("Os identificadores das alternativas são inválidos. Reabra a edição.", [nameof(AlternativaIds)]);
            }
        }
        else if (Tipo == "VerdadeiroFalso")
        {
            if (!GabaritoVf.HasValue)
            {
                yield return new ValidationResult("Selecione o gabarito verdadeiro ou falso.", [nameof(GabaritoVf)]);
            }
        }
        else
        {
            yield return new ValidationResult("Selecione um tipo de questão válido.", [nameof(Tipo)]);
        }
    }
}
