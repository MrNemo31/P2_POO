using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LiteArt.Poo.Models.ViewModels;

public record AlunoOpcao(int Id, string Nome);
public record AlternativaResposta(int Id, string Texto);

public class QuestaoResolucaoViewModel
{
    public int Id { get; init; }
    public string Enunciado { get; init; } = "";
    public bool Ativa { get; init; }
    public string Tipo { get; init; } = "MultiplaEscolha";
    public string TipoTexto => Tipo == "MultiplaEscolha" ? "Múltipla escolha" : "Verdadeiro/falso";
    public IReadOnlyList<MateriaOpcao> Materias { get; init; } = [];
    public IReadOnlyList<AlternativaResposta> Alternativas { get; init; } = [];
}

public class ResolverViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Selecione um aluno.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um aluno válido.")]
    public int? AlunoId { get; set; }
    public int? MateriaId { get; set; }
    [Required(ErrorMessage = "Selecione uma resposta.")]
    public string? Resposta { get; set; }
    // Resultado fornecido pelo back-end após o envio. Nulo mantém o feedback oculto.
    [BindNever]
    public bool? Acertou { get; set; }
    [BindNever]
    public QuestaoResolucaoViewModel? Questao { get; set; }
    [BindNever]
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
