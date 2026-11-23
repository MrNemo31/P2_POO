using System.Net.Mail;
using System;
using System.Collections.Generic;

namespace LiteArt.Poo.Models.Entities
{
    public class Aluno
    {
        public int IdAluno {get; private set;}    
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public List<Tentativa> Tentativas { get; private set; } = new List<Tentativa>();
        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public Aluno(string Nome, string Email)
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                throw new ArgumentException("O nome do aluno não pode ser nulo ou vazio.", nameof(Nome));
            }
            else
            {
                this.Nome = Nome.Trim();
            }
            if (ValidarEmail(Email))
                this.Email = Email.Trim();
            else
                throw new ArgumentException("O email do aluno não é válido.", nameof(Email));
        }
        public void AdicionarTentativa(Tentativa tentativa)
        {
            if (tentativa == null)
                throw new ArgumentNullException(nameof(tentativa), "A tentativa não pode ser nula.");

            if (ReferenceEquals(tentativa.Aluno, this) == false)
                throw new InvalidOperationException("A tentativa não pertence a este aluno.");    
            Tentativas.Add(tentativa);
        }
    }
}