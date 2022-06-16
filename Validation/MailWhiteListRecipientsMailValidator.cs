using System.Net.Mail;
using System.Text.RegularExpressions;

namespace POO2_Ex9.Validation;

public class MailWhiteListRecipientsMailValidator : IMailValidator
{
    private readonly Regex _whiteRegex = new("@(cpnv.ch|vd.ch)$");

    public bool IsValid(MailMessage mail)
    {
        return AreInWhiteList(mail.To);
    }

    private bool AreInWhiteList(MailAddressCollection addresses)
    {
        return addresses.All(address => IsInWhiteList(address.Address));
    }

    private bool IsInWhiteList(string address)
    {
        return _whiteRegex.IsMatch(address);
    }
}