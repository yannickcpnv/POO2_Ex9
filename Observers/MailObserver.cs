using System.Net.Mail;
using System.Text;
using POO2_Ex9.Validation;

namespace POO2_Ex9.Observers;

public class MailObserver : IObserver<MailMessage>
{
    private readonly MailValidator _mailValidator = new();

    private int _receivedCount;
    private int _rejectedBytes;
    private int _rejectedCount;
    private int _storedBytes;

    public void OnNext(MailMessage mail)
    {
        Receive(mail);

        if (_mailValidator.IsValid(mail))
            Store(mail);
        else
            Reject(mail);

        WriteStatsInFile();
    }

    public void OnError(Exception error)
    {
        Console.Error.WriteLine(error.Message);
    }

    public void OnCompleted()
    {
        Console.WriteLine("Received {0} emails", _receivedCount);
    }

    private void Receive(MailMessage mail)
    {
        Console.WriteLine($"Received mail : {mail.From} {mail.To[0]}");
        _receivedCount++;
    }

    private void Store(MailMessage mail)
    {
        Console.WriteLine($"Stored mail : {mail.From} {mail.To[0]}");
        _storedBytes += Encoding.Default.GetByteCount(mail.Body);

        foreach (MailAddress address in mail.To)
            WriteBodyInFile(mail, address);
    }

    private static void WriteBodyInFile(MailMessage mail, MailAddress address)
    {
        string fileName = Path.Combine("data", address.Address, $"{DateTime.Now.Ticks}.eml");
        var file = new FileInfo(fileName);
        file.Directory?.Create();
        File.WriteAllText(file.FullName, mail.Body);
    }

    private void Reject(MailMessage mail)
    {
        Console.WriteLine($"Rejected mail : {mail.From} {mail.To[0]}");
        _rejectedCount++;
        _rejectedBytes += Encoding.Default.GetByteCount(mail.Body);
    }

    private void WriteStatsInFile()
    {
        using var file = new StreamWriter("data/stats.txt");
        file.WriteLine($"Received count  : {_receivedCount}");
        file.WriteLine($"Rejected count  : {_rejectedCount}");
        file.WriteLine($"Stored bytes    : {_storedBytes}");
        file.WriteLine($"Rejected bytes  : {_rejectedBytes}");
        file.WriteLine($"Spam ratio      : {_rejectedCount * 100 / _receivedCount}%");
        file.WriteLine($"Spam bytes rate : {(float)_rejectedBytes / _storedBytes:0.00}");
    }
}