
namespace LiteArt.Poo.Models.Entities
{
    public abstract class  Questao
    {
        public int IdQuestao { get; protected set; }
        public string Enunciado { get; private set; } = string.Empty;
        public bool Ativa { get; private set; } = true;
        public Administrador Administrador { get; private set; }
        private List<Materia> _materias = new List<Materia>();
        public IReadOnlyList<Materia> Materias => this._materias.AsReadOnly();
     
        protected Questao(string Enunciado, Administrador Administrador, List<Materia> Materias)
    {
        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            throw new ArgumentException("O enunciado não pode ser nulo ou vazio.", nameof(Enunciado));
        }
        else
        {
            this.Enunciado = Enunciado.Trim();
        }
        if (Administrador == null)
            throw new ArgumentNullException(nameof(Administrador), "O administrador não pode ser nulo.");
        else
            this.Administrador = Administrador;
        if (Materias == null || Materias.Count == 0)
            throw new ArgumentException("A lista de matérias não pode ser nula ou vazia.", nameof(Materias));
        else
            for (int i = 0; i < Materias.Count; i++)
            {
                if (Materias[i] == null)
                    throw new ArgumentException($"A matéria na posição {i} não pode ser nula.", nameof(Materias));
            }
        this._materias = new List<Materia>(Materias);
    
    }
    protected Questao(string Enunciado, Administrador Administrador, List<Materia> Materias, int IdQuestao) : this(Enunciado, Administrador, Materias)
    {
        if (IdQuestao <= 0)
            throw new ArgumentException("O Id da questão deve ser maior que zero.", nameof(IdQuestao));
        this.IdQuestao = IdQuestao;
    }

    public abstract bool AvaliarResposta(string Resposta);
    
        public void Ativar()
        {
            Ativa = true;
        }

        public void Desativar()
        {
            Ativa = false;
        }
        public void EditarDados (string novoEnunciado, List<Materia> novasMaterias)
        {
            if (string.IsNullOrWhiteSpace(novoEnunciado))
                throw new ArgumentException("O enunciado não pode ser nulo ou vazio.", nameof(novoEnunciado));
            if (novasMaterias == null || novasMaterias.Count == 0)
                throw new ArgumentException("A lista de matérias não pode ser nula ou vazia.", nameof(novasMaterias));
            for (int i = 0; i < novasMaterias.Count; i++)
            {
                if (novasMaterias[i] == null)
                    throw new ArgumentException($"A matéria na posição {i} não pode ser nula.", nameof(novasMaterias));
            }
            this.Enunciado = novoEnunciado.Trim();
            this._materias = new List<Materia>(novasMaterias);
        }
    }
}
