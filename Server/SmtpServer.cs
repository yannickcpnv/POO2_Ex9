using System.Text.RegularExpressions;
using netDumbster.smtp;

namespace POO2_Ex9.Server;

public class SmtpServer
{
    private static Regex? _whiteRegex;
    private static Regex? _badWordsRegex;
    private readonly int _port;
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
        _server!.Stop();
    }

    public void WaitForReceivingMessage()
    {
        const string storeLocation = "data";
        const string statsFilename = "data/stats.txt";

        _server!.MessageReceived += (_, args) =>
        {
            SmtpMessage mail = args.Message;

            Console.WriteLine($"Received mail: {mail.FromAddress} {mail.ToAddresses[0]}");
            _receivedCount++;

            if (HasBadWords(mail) || HasInvalidRecipient(mail))
            {
                Console.WriteLine($"Rejected mail: {mail.FromAddress} {mail.ToAddresses[0]}");
                _rejectedCount++;
            }
            else
            {
                Console.WriteLine($"Stored mail  : {mail.FromAddress} {mail.ToAddresses[0]}");

                foreach (EmailAddress address in mail.ToAddresses)
                {
                    string fileName = Path.Combine(storeLocation, address.Address, $"{DateTime.Now.Ticks}.eml");
                    var file = new FileInfo(fileName);
                    file.Directory!.Create();
                    File.WriteAllText(file.FullName, mail.Data);
                }
            }

            WriteInFile(statsFilename);
        };
    }

    private static bool HasBadWords(SmtpMessage mail)
    {
        return mail.MessageParts.Any(part => _badWordsRegex!.IsMatch(part.BodyData));
    }

    private static bool HasInvalidRecipient(SmtpMessage mail)
    {
        return !mail.ToAddresses.Any(address => _whiteRegex!.IsMatch(address.Address));
    }

    private void WriteInFile(string statsFilename)
    {
        using var file = new StreamWriter(statsFilename);
        file.WriteLine($"Received count: {_receivedCount}");
        file.WriteLine($"Rejected count: {_rejectedCount}");
        file.WriteLine($"Spam ratio: {_rejectedCount * 100 / _receivedCount}%");
    }
}