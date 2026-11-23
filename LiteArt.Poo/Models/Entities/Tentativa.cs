namespace LiteArt.Poo.Models.Entities
{
    public class Tentativa
    {
        public int IdTentativa { get; private set; }
        public Aluno Aluno { get; private set; }
        public Questao Questao { get; private set; }
        public string Resposta { get; private set; } = string.Empty;
        public bool Acertou { get; private set; }
        public DateTime DataHora { get; private set; }
    
        public Tentativa(Aluno Aluno, Questao Questao, string Resposta)
        {
            this.Aluno = Aluno ?? throw new ArgumentNullException(nameof(Aluno), "O aluno não pode ser nulo.");
            this.Questao = Questao ?? throw new ArgumentNullException(nameof(Questao), "A questão não pode ser nula.");
            if (string.IsNullOrWhiteSpace(Resposta))
                throw new ArgumentException("A resposta não pode ser nula ou vazia.", nameof(Resposta));
            if (!Questao.Ativa)
                throw new InvalidOperationException("Não é possível registrar uma tentativa para uma questão inativa.");   
            this.Resposta = Resposta;
            this.Acertou = Questao.AvaliarResposta(Resposta);
            this.DataHora = DateTime.Now;
        }
    }
}