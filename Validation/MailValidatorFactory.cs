namespace POO2_Ex9.Validation;

public static class MailValidatorFactory
{
    public static IMailValidator Create(string validationType)
    {
        return validationType switch
        {
            "BadWords" => new BadWordsValidator(),
            "WhiteListRecipients" => new WhiteListRecipientsValidator(),
            "AttachmentsExtension" => new AttachmentsExtensionValidator(),
            _ => throw new ArgumentException("Invalid validator type")
        };
    }
}