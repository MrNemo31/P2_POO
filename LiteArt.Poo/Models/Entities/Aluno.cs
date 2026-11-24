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
        private List<Tentativa> _tentativas = new List<Tentativa>();
        public IReadOnlyList<Tentativa> Tentativas => this._tentativas.AsReadOnly();
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
        // CONSTRUTOR DE UM NOVO ALUNO
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
        public Aluno (string Nome, string Email, int IdAluno) : this(Nome, Email)
        {
            if (IdAluno <= 0)
                throw new ArgumentException("O Id do aluno deve ser maior que zero.", nameof(IdAluno));
            this.IdAluno = IdAluno;
        }
        public void AdicionarTentativa(Tentativa tentativa)
        {
            if (tentativa == null)
                throw new ArgumentNullException(nameof(tentativa), "A tentativa não pode ser nula.");

            if (ReferenceEquals(tentativa.Aluno, this) == false)
                throw new InvalidOperationException("A tentativa não pertence a este aluno.");    
            this._tentativas.Add(tentativa);
        }
        public void EditarDados(string novoNome, string novoEmail)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("O nome do aluno não pode ser nulo ou vazio.", nameof(novoNome));
            if (ValidarEmail(novoEmail))
            {
                this.Nome = novoNome.Trim();
                this.Email = novoEmail.Trim();
            }
            else
                throw new ArgumentException("O email do aluno não é válido.", nameof(novoEmail));
        }    
    }
}