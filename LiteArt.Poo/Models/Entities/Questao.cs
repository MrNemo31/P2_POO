
namespace LiteArt.Poo.Models.Entities
{
    public abstract class  Questao
    {
        public int IdQuestao { get; private set; }
        public string Enunciado { get; private set; } = string.Empty;
        public bool Ativa { get; private set; } = true;
        public Administrador Administrador { get; private set; }
        public List<Materia> Materias { get; private set; } = new List<Materia>();
     
        protected Questao(string Enunciado, Administrador Administrador, List<Materia> Materias)
    {
        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            throw new ArgumentException("O enunciado não pode ser nulo ou vazio.", nameof(Enunciado));
        }
        else
        {
            this.Enunciado = Enunciado;
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
        this.Materias = new List<Materia>(Materias);
    
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
    }
}
