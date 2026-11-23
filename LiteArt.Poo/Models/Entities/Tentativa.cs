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
    
        public Tentativa(Aluno aluno, Questao Questao, string Resposta)
        {
            this.Aluno = aluno ?? throw new ArgumentNullException(nameof(aluno), "O aluno não pode ser nulo.");
            this.Questao = Questao ?? throw new ArgumentNullException(nameof(Questao), "A questão não pode ser nula.");
            this.Resposta = Resposta ?? throw new ArgumentNullException(nameof(Resposta), "A resposta não pode ser nula.");
            if (Questao.AvaliarResposta(Resposta))
            {
                this.Acertou = true;
            }
            else
            {
                this.Acertou = false;
            }
            this.DataHora = DateTime.Now;
        }
    }
}