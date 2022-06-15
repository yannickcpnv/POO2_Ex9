using System.Net.Mail;

namespace POO2_Ex9.Validation;

public class MailAttachmentsExtensionValidation : IValidation
{
    public bool IsValid(MailMessage mail)
    {
        return mail.Attachments.Any(attachment => attachment.ContentType.MediaType.Contains("zip"));
    }
}