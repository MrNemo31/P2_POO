using System.Net.Mail;
using System;
using System.Collections.Generic;

namespace LiteArt.Poo.Models.Entities
{
    public class Aluno
    {
        public string IdAluno {get; set;}    
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<Tentativa> Tentativas { get; private set; } = new List<Tentativa>();
        public bool ValidarEmail()
        {
            try
            {
                var mailAddress = new MailAddress(Email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}