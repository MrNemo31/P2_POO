
namespace LiteArt.Poo.Models.Entities
{
    public class Questao
    {
        public int IdQuestao { get; set; }

        public string Enunciado { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;

        public string TipoQuestao { get; set; } = string.Empty;

        public bool? RespostaCorreta { get; set; }

        public int IdAdministrador { get; set; }
    }
}
