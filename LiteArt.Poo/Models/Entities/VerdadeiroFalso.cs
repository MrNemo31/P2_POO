namespace LiteArt.Poo.Models.Entities
{
    public class VerdadeiroFalso : Questao
    {
        public bool RespostaCorreta { get; private set; }

        public VerdadeiroFalso(string Enunciado, bool RespostaCorreta, Administrador Administrador, List<Materia> Materias) : base(Enunciado, Administrador, Materias)
        {
            this.RespostaCorreta = RespostaCorreta;
        }

        public override bool AvaliarResposta(string Resposta)
        {
            if (string.IsNullOrWhiteSpace(Resposta))
                return false;

            if (!bool.TryParse(Resposta, out bool RespostaUsuario))
                return false;

            return RespostaUsuario == this.RespostaCorreta;
        }
    }
}