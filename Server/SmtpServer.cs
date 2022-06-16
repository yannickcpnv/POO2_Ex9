using System.Net.Mail;
using netDumbster.smtp;
using POO2_Ex9.Observers;
using POO2_Ex9.Validation;

namespace POO2_Ex9.Server;

public class SmtpServer
{
    private readonly List<IMailObserver> _mailObservers;
    private readonly MailValidator _mailValidator;
    private readonly int _port;

    private SimpleSmtpServer? _server;

    public SmtpServer(int port, MailValidator mailValidator)
    {
        _port = port;
        _mailValidator = mailValidator;
        _mailObservers = new List<IMailObserver>();
    }

    public void Subscribe(IMailObserver observer)
    {
        if (!_mailObservers.Contains(observer))
            _mailObservers.Add(observer);
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
            MailMessage mail = MailMessageMimeParser.ParseMessage(rawMessage);

            Receive(mail);

            if (_mailValidator.IsValid(mail))
                Store(mail);
            else
                Reject(mail);
        };
    }

    private void Receive(MailMessage mail)
    {
        _mailObservers.ForEach(o => o.OnReceive(mail));
    }

    private void Store(MailMessage mail)
    {
        _mailObservers.ForEach(o => o.OnStore(mail));

        mail.To.ToList().ForEach(a => WriteBodyInFile(mail, a));
    }

    private void Reject(MailMessage mail)
    {
        _mailObservers.ForEach(o => o.OnReject(mail));
    }

    private static void WriteBodyInFile(MailMessage mail, MailAddress address)
    {
        string fileName = Path.Combine("data", address.Address, $"{DateTime.Now.Ticks}.eml");
        var file = new FileInfo(fileName);
        file.Directory?.Create();
        File.WriteAllText(file.FullName, mail.Body);
    }
}