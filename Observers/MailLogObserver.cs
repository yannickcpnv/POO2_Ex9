using System.Net.Mail;

namespace POO2_Ex9.Observers;

public class MailLogObserver : IMailObserver
{
    public void OnReceive(MailMessage mail)
    {
        Console.WriteLine($"Received mail : {mail.From} {mail.To[0]}");
    }

    public void OnStore(MailMessage mail)
    {
        Console.WriteLine($"Stored mail : {mail.From} {mail.To[0]}");
    }

    public void OnReject(MailMessage mail)
    {
        Console.WriteLine($"Rejected mail : {mail.From} {mail.To[0]}");
    }
}