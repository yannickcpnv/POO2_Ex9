using System.Net.Mail;
using System.Text.RegularExpressions;

namespace POO2_Ex9.Validation;

public class MailWhiteListRecipientsValidation : IValidation
{
    private readonly Regex _whiteRegex = new("@(cpnv.ch|vd.ch)$");

    public bool IsValid(MailMessage mail)
    {
        return mail.To.All(address => _whiteRegex.IsMatch(address.Address));
    }
}