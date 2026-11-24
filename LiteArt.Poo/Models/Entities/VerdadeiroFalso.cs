namespace LiteArt.Poo.Models.Entities
{
    public class VerdadeiroFalso : Questao
    {
        public bool RespostaCorreta { get; private set; }

        public VerdadeiroFalso(string Enunciado, bool RespostaCorreta, Administrador Administrador, List<Materia> Materias) : base(Enunciado, Administrador, Materias)
        {
            this.RespostaCorreta = RespostaCorreta;
        }

        public VerdadeiroFalso(string Enunciado, bool RespostaCorreta, Administrador Administrador, List<Materia> Materias, int IdQuestao) : this(Enunciado, RespostaCorreta, Administrador, Materias)
        {
            if (IdQuestao <= 0)
                throw new ArgumentException("O Id da questão deve ser maior que zero.", nameof(IdQuestao));
            this.IdQuestao = IdQuestao;
        }
        public override bool AvaliarResposta(string Resposta)
        {
            if (string.IsNullOrWhiteSpace(Resposta))
                return false;

            if (!bool.TryParse(Resposta, out bool RespostaUsuario))
                return false;

            return RespostaUsuario == this.RespostaCorreta;
        }
        public void EditarRespostaCorreta(bool novaRespostaCorreta)
        {
            this.RespostaCorreta = novaRespostaCorreta;
        }
    }
}