using System.Net.Mail;
using System.Text;
using netDumbster.smtp;
using POO2_Ex9.Validation;

namespace POO2_Ex9.Server;

public class SmtpServer
{
    private readonly MailValidator _mailValidator;
    private readonly int _port;

    private int _receivedCount;
    private int _rejectedBytes;
    private int _rejectedCount;
    private SimpleSmtpServer? _server;
    private int _storedBytes;

    public SmtpServer(int port)
    {
        _port = port;
        _receivedCount = 0;
        _rejectedCount = 0;
        _rejectedBytes = 0;
        _storedBytes = 0;
        _mailValidator = new MailValidator();
    }

    public void Start()
    {
        _server = SimpleSmtpServer.Start(_port);
    }

    public void Stop()
    {
        _server?.Stop();
    }

    public void WaitReceivingMessages()
    {
        _server!.MessageReceived += (_, args) =>
        {
            string rawMessage = args.Message.Data.TrimEnd('\r', '\n');
            MailMessage mailMessage = MailMessageMimeParser.ParseMessage(rawMessage);

            Receive(mailMessage);

            if (_mailValidator.IsValid(mailMessage))
                Store(mailMessage);
            else
                Reject(mailMessage);

            WriteInFile();
        };
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
        {
            string fileName = Path.Combine("data", address.Address, $"{DateTime.Now.Ticks}.eml");
            var file = new FileInfo(fileName);
            file.Directory?.Create();
            File.WriteAllText(file.FullName, mail.Body);
        }
    }

    private void Reject(MailMessage mail)
    {
        Console.WriteLine($"Rejected mail : {mail.From} {mail.To[0]}");
        _rejectedCount++;
        _rejectedBytes += Encoding.Default.GetByteCount(mail.Body);
    }

    private void WriteInFile()
    {
        using var file = new StreamWriter("data/stats.txt");
        file.WriteLine($"Received count: {_receivedCount}");
        file.WriteLine($"Stored bytes  : {_storedBytes}");
        file.WriteLine($"Rejected count: {_rejectedCount}");
        file.WriteLine($"Rejected bytes: {_rejectedBytes}");
        file.WriteLine($"Spam ratio: {_rejectedCount * 100 / _receivedCount}%");
    }
}