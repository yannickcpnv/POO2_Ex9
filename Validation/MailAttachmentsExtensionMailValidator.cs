using System.Net.Mail;
using System.Text.RegularExpressions;

namespace POO2_Ex9.Validation;

public class MailAttachmentsExtensionMailValidator : IMailValidator
{
    private readonly Regex _badExtensions = new(@"^(zip)$");

    public bool IsValid(MailMessage mail)
    {
        return !HaveBadExtension(mail.Attachments);
    }

    private bool HaveBadExtension(AttachmentCollection attachments)
    {
        return attachments.Any(attachment => HasBadExtension(attachment.ContentType.MediaType));
    }

    private bool HasBadExtension(string mediaType)
    {
        return _badExtensions.IsMatch(mediaType);
    }
}