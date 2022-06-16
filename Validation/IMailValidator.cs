using System.Net.Mail;

namespace POO2_Ex9.Validation;

public interface IMailValidator
{
    public bool IsValid(MailMessage mail);
}