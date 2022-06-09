using System.Text.RegularExpressions;
using netDumbster.smtp;

namespace POO2_Ex9;

internal static class Program
{
    private static Regex? _whiteRegex;
    private static Regex? _badWordsRegex;

    private static void Main()
    {
        var exitEvent = new ManualResetEvent(false);

        WaitForExitInput(exitEvent);

        const int smtpServerPort = 3325;
        const string storeLocation = "data";
        const string statsFilename = "data/stats.txt";
        _whiteRegex = new Regex("@(cpnv.ch|vd.ch)$");
        _badWordsRegex = new Regex(string.Join("|", File.ReadLines("bad_words_list.txt")));

        var receivedCount = 0;
        var rejectedCount = 0;

        SimpleSmtpServer smtpServer = SimpleSmtpServer.Start(smtpServerPort);
        smtpServer.MessageReceived += (_, args) =>
        {
            SmtpMessage mail = args.Message;

            Console.WriteLine($"Received mail: {mail.FromAddress} {mail.ToAddresses[0]}");
            receivedCount++;

            if (HasBadWords(mail) || HasInvalidRecipient(mail))
            {
                Console.WriteLine($"Rejected mail: {mail.FromAddress} {mail.ToAddresses[0]}");
                rejectedCount++;
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

            using (var file = new StreamWriter(statsFilename))
            {
                file.WriteLine($"Received count: {receivedCount}");
                file.WriteLine($"Rejected count: {rejectedCount}");
                file.WriteLine($"Spam ratio: {rejectedCount * 100 / receivedCount}%");
            }
        };

        exitEvent.WaitOne();
        smtpServer.Stop();
    }

    private static void WaitForExitInput(EventWaitHandle exitEvent)
    {
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            exitEvent.Set();
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
}