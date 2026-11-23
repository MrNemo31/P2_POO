using System.Net.Mail;
using System;

namespace LiteArt.Poo.Models.Entities
{
    public class Administrador
    {
        public int IdAdministrador { get; private set; }

        public string Nome { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

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

        public Administrador(string Nome, string Email)
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                throw new ArgumentException("O nome do administrador não pode ser nulo ou vazio.", nameof(Nome));
            }
            else
            {
                this.Nome = Nome.Trim();
            }

            if (ValidarEmail(Email))
                this.Email = Email.Trim();
            else
                throw new ArgumentException("O email do administrador não é válido.", nameof(Email));
        }
    }
}