using System.Net.Mail;
using System.Text.RegularExpressions;

namespace POO2_Ex9.Validation;

public class MailBadWordsValidation : IValidation
{
    private readonly Regex _badWordsRegex = new(string.Join("|", File.ReadLines("bad_words_list.txt")));

    public bool IsValid(MailMessage mail)
    {
        return !_badWordsRegex.IsMatch(mail.Body);
    }
}