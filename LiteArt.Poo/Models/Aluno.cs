using System.Net.Mail;
using System;
using System.Collections.Generic;

class Aluno
{
    public string ID {get; set;}    
    public string Nome { get; set; }
    public string Email { get; set; }
    public List<Tentativas> Tentativas { get; set; } = null;
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
