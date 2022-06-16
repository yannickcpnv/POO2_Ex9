using System.Net.Mail;

namespace POO2_Ex9.Validation;

public class MailValidator : IMailValidator
{
    private readonly List<IMailValidator> _validations;

    public MailValidator()
    {
        _validations = new List<IMailValidator>();
    }

    public bool IsValid(MailMessage mail)
    {
        return _validations.All(v => v.IsValid(mail));
    }

    public void Add(IMailValidator validator)
    {
        _validations.Add(validator);
    }
}