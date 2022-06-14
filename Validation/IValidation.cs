using System.Net.Mail;

namespace POO2_Ex9.Validation;

public interface IValidation
{
    public bool IsValid(MailMessage mail);
}