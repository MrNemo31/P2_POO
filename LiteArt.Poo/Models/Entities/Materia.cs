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
            if (Descricao != null && Descricao.Length > 500)
                throw new ArgumentException("A descrição da matéria não pode ter mais de 500 caracteres.", nameof(Descricao));
            this.Descricao = Descricao;
            if (Administrador == null)
                throw new ArgumentNullException(nameof(Administrador), "O administrador não pode ser nulo.");
            else 
                this.Administrador = Administrador;
        }
        public Materia(string Nome, string? Descricao, Administrador Administrador, int IdMateria) : this(Nome, Descricao, Administrador)
        {
            if (IdMateria <= 0)
                throw new ArgumentException("O Id da matéria deve ser maior que zero.", nameof(IdMateria));
            this.IdMateria = IdMateria;
        }
        public void EditarDados(string novoNome, string? novaDescricao, Administrador novoAdministrador)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("O nome da matéria não pode ser nulo ou vazio.", nameof(novoNome));
            if (novaDescricao != null && novaDescricao.Length > 500)
                throw new ArgumentException("A descrição da matéria não pode ter mais de 500 caracteres.", nameof(novaDescricao));
            if (novoAdministrador == null)
                throw new ArgumentNullException(nameof(novoAdministrador), "O administrador não pode ser nulo.");
            this.Nome = novoNome.Trim();
            this.Descricao = novaDescricao?.Trim();
            this.Administrador = novoAdministrador;
        }  
    }
}