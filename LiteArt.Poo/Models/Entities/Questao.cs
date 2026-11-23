
namespace LiteArt.Poo.Models.Entities
{
    public abstract class  Questao
    {
        public int IdQuestao { get; private set; }

        public string Enunciado { get; private set; } = string.Empty;

        public bool Ativa { get; private set; } = true;


    protected Questao(string Enunciado)
    {
        if (string.IsNullOrWhiteSpace(Enunciado))
        {
            throw new ArgumentException("O enunciado não pode ser nulo ou vazio.", nameof(Enunciado));
        }
        else
        {
            this.Enunciado = Enunciado;
        }
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
