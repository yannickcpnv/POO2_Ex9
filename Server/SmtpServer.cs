using System.Text.RegularExpressions;
using netDumbster.smtp;

namespace POO2_Ex9.Server;

public class SmtpServer
{
    private readonly Regex? _badWordsRegex;
    private readonly int _port;
    private readonly Regex? _whiteRegex;

    private int _receivedCount;
    private int _rejectedCount;
    private SimpleSmtpServer? _server;

    public SmtpServer(int port)
    {
        _port = port;
        _receivedCount = 0;
        _rejectedCount = 0;

        _whiteRegex = new Regex("@(cpnv.ch|vd.ch)$");
        _badWordsRegex = new Regex(string.Join("|", File.ReadLines("bad_words_list.txt")));
    }

    public void Start()
    {
        _server = SimpleSmtpServer.Start(_port);
    }

    public void Stop()
    {
        _server?.Stop();
    }

    public void WaitReceivingMessage()
    {
        _server!.MessageReceived += (_, args) =>
        {
            SmtpMessage mail = args.Message;

            Receive(mail);

            if (IsInvalid(mail))
                Reject(mail);
            else
                Store(mail);

            WriteInFile();
        };
    }

    private void Receive(SmtpMessage mail)
    {
        Console.WriteLine($"Received mail: {mail.FromAddress} {mail.ToAddresses[0]}");
        _receivedCount++;
    }

    private bool IsInvalid(SmtpMessage mail)
    {
        return HasBadWords(mail) || HasInvalidRecipient(mail);
    }

    private bool HasBadWords(SmtpMessage mail)
    {
        return mail.MessageParts.Any(part => _badWordsRegex!.IsMatch(part.BodyData));
    }

    private bool HasInvalidRecipient(SmtpMessage mail)
    {
        return !mail.ToAddresses.Any(address => _whiteRegex!.IsMatch(address.Address));
    }

    private static void Store(SmtpMessage mail)
    {
        Console.WriteLine($"Stored mail  : {mail.FromAddress} {mail.ToAddresses[0]}");

        foreach (EmailAddress address in mail.ToAddresses)
        {
            string fileName = Path.Combine("data", address.Address, $"{DateTime.Now.Ticks}.eml");
            var file = new FileInfo(fileName);
            file.Directory?.Create();
            File.WriteAllText(file.FullName, mail.Data);
        }
    }

    private void Reject(SmtpMessage mail)
    {
        Console.WriteLine($"Rejected mail: {mail.FromAddress} {mail.ToAddresses[0]}");
        _rejectedCount++;
    }

    private void WriteInFile()
    {
        using var file = new StreamWriter("data/stats.txt");
        file.WriteLine($"Received count: {_receivedCount}");
        file.WriteLine($"Rejected count: {_rejectedCount}");
        file.WriteLine($"Spam ratio: {_rejectedCount * 100 / _receivedCount}%");
    }
}