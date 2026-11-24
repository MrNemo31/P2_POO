using System.ComponentModel.DataAnnotations;

namespace LiteArt.Poo.Models.ViewModels
{
    public class MateriaFormularioViewModel
    {
        public int IdMateria { get; set; }
        [Required(ErrorMessage = "Informe o nome da matéria.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        [Range(1, int.MaxValue,
            ErrorMessage = "Informe um ID de administrador maior que zero.")]
        public int IdAdministrador { get; set; }
    }
}