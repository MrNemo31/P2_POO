namespace LiteArt.Poo.Models.Entities
{
    public class Alternativa
    {
        public int IdAlternativa { get; private set; }

        public string Texto { get; private set; } = string.Empty;

        public bool Correta { get; private set; }

        public Alternativa(string Texto, bool Correta)
        {
            if (string.IsNullOrWhiteSpace(Texto))
            {
                throw new ArgumentException("O texto da alternativa não pode ser nulo ou vazio.", nameof(Texto));
            }
            else
            {
                this.Texto = Texto;
            }
            this.Correta = Correta;
        }
    }
}