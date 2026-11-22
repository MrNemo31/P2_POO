namespace LiteArt.Poo.Models.Entities
{
    public class Materia
    {
        public int IdMateria { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public int IdAdministrador { get; set; }
    }
}