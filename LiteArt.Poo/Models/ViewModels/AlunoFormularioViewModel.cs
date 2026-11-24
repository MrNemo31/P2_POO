using System.ComponentModel.DataAnnotations;

namespace LiteArt.Poo.Models.ViewModels
{
    public class AlunoFormularioViewModel
    {
        public int IdAluno { get; set; }

        [Required(ErrorMessage = "Informe o nome do aluno.")]
        [StringLength(100,
            ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail do aluno.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(150,
            ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;
    }
}