namespace LiteArt.Poo.Models.Entities
{
    public class Materia
    {
        public int IdMateria { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string? Descricao { get; private set; }
        public Administrador Administrador { get; private set; }
        public Materia(string Nome, string? Descricao, Administrador Administrador)
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                throw new ArgumentException("O nome da matéria não pode ser nulo ou vazio.", nameof(Nome));
            }
            else
            {
                this.Nome = Nome.Trim();
            }
            this.Descricao = Descricao;
            if (Administrador == null)
                throw new ArgumentNullException(nameof(Administrador), "O administrador não pode ser nulo.");
            else 
                this.Administrador = Administrador;
        }
    }
}