using System.Net.Mail;
using System.Text.RegularExpressions;

namespace POO2_Ex9.Validation;

public class MailBadWordsMailValidator : IMailValidator
{
    private readonly Regex _badWordsRegex = new(string.Join("|", File.ReadLines("bad_words_list.txt")));

    public bool IsValid(MailMessage mail)
    {
        return !HasBadWords(mail.Body);
    }

    private bool HasBadWords(string body)
    {
        return _badWordsRegex.IsMatch(body);
    }
}