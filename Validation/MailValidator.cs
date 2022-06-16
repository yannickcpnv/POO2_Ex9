using System.Net.Mail;

namespace POO2_Ex9.Validation;

public class MailValidator : IMailValidator
{
    private readonly List<IMailValidator> _validations;

    public MailValidator()
    {
        _validations = new List<IMailValidator>
        {
            new MailBadWordsMailValidator(),
            new MailWhiteListRecipientsMailValidator(),
            new MailAttachmentsExtensionMailValidator()
        };
    }

    public bool IsValid(MailMessage mail)
    {
        return _validations.All(validation => validation.IsValid(mail));
    }
}