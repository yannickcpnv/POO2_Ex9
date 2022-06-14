using System.Net.Mail;

namespace POO2_Ex9.Validation;

public class MailValidator: IValidation
{
    private readonly List<IValidation> _validations;
    
    public MailValidator()
    {
        _validations = new List<IValidation>
        {
            new MailBadWordsValidation(),
            new MailInvalidRecipientValidation()
        };
    }
    
    public bool IsValid(MailMessage mail)
    {
        return _validations.All(validation => validation.IsValid(mail));
    }
}