namespace LiteArt.Poo.Models.Entities
{
    
    public class MultiplaEscolha : Questao
    {
        public List<Alternativa> Opcoes { get; private set; } = new List<Alternativa>();

        public MultiplaEscolha(string Enunciado, List<Alternativa> Opcoes) : base(Enunciado)
        {
            int contador = 0;
            
            if (Opcoes == null || Opcoes.Count < 2)
            {
                throw new ArgumentException("Uma questão de múltipla escolha deve ter pelo menos duas opções.", nameof(Opcoes));
            }
            for (int i = 0; i < Opcoes.Count; i++)
            {
                if (Opcoes[i].Correta == true)
                {
                    contador += 1;
                }

            }
            if (contador != 1)
            {
                throw new ArgumentException("A questão de múltipla escolha deve ter apenas uma opção correta.", nameof(Opcoes));
            }
            this.Opcoes = Opcoes;
        }

        public override bool AvaliarResposta(Alternativa Resposta)
        {
            if (Resposta == null)
                return false;

            for (int i = 0; i < Opcoes.Count; i++)
            {
                if (Opcoes[i].IdAlternativa == Resposta.IdAlternativa)
                {
                    return Opcoes[i].Correta;
                }
            }

            return false;
                }
            }
        }