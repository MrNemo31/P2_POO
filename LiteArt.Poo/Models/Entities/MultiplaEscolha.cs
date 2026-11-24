namespace LiteArt.Poo.Models.Entities
{
    
    public class MultiplaEscolha : Questao
    {
        private List<Alternativa> _opcoes = new List<Alternativa>();
        public IReadOnlyList<Alternativa> Opcoes => this._opcoes.AsReadOnly();
        public MultiplaEscolha(string Enunciado, List<Alternativa> Opcoes, Administrador Administrador, List<Materia> Materias) : base(Enunciado, Administrador, Materias)
        {
            int contador = 0;
            
            if (Opcoes == null || Opcoes.Count < 2)
            {
                throw new ArgumentException("Uma questão de múltipla escolha deve ter pelo menos duas opções.", nameof(Opcoes));
            }
            for (int i = 0; i < Opcoes.Count; i++)
            {
                if (Opcoes[i] == null)
                {
                    throw new ArgumentException($"A opção na posição {i} não pode ser nula.", nameof(Opcoes));
                }
                
                if (Opcoes[i].Correta)
                {
                    contador += 1;
                }

            }
            if (contador != 1)
            {
                throw new ArgumentException("A questão de múltipla escolha deve ter apenas uma opção correta.", nameof(Opcoes));
            }
            this._opcoes = new List<Alternativa>(Opcoes);
        }
        public MultiplaEscolha(string Enunciado, List<Alternativa> Opcoes, Administrador Administrador, List<Materia> Materias, int IdQuestao) : this(Enunciado, Opcoes, Administrador, Materias)
        {
            if (IdQuestao <= 0)
                throw new ArgumentException("O Id da questão deve ser maior que zero.", nameof(IdQuestao));
            this.IdQuestao = IdQuestao;
        }

        public override bool AvaliarResposta(string Resposta)
        {
            if (string.IsNullOrWhiteSpace(Resposta))
                return false;

            if (!int.TryParse(Resposta, out int IdAlternativa))
                return false;

            for (int i = 0; i < Opcoes.Count; i++)
            {
                if (Opcoes[i].IdAlternativa == IdAlternativa)
                {
                    return Opcoes[i].Correta;
                }
            }

            return false;
        }

        public void EditarOpcoes(List<Alternativa> novasOpcoes)
        {
            int contador = 0;

            if (novasOpcoes == null || novasOpcoes.Count < 2)
            {
                throw new ArgumentException("Uma questão de múltipla escolha deve ter pelo menos duas opções.", nameof(novasOpcoes));
            }
            for (int i = 0; i < novasOpcoes.Count; i++)
            {
                if (novasOpcoes[i] == null)
                {
                    throw new ArgumentException($"A opção na posição {i} não pode ser nula.", nameof(novasOpcoes));
                }

                if (novasOpcoes[i].Correta)
                {
                    contador += 1;
                }

            }
            if (contador != 1)
            {
                throw new ArgumentException("A questão de múltipla escolha deve ter apenas uma opção correta.", nameof(novasOpcoes));
            }
            this._opcoes = new List<Alternativa>(novasOpcoes);
        }        
    }
}