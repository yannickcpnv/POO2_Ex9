using System.Net.Mail;
using netDumbster.smtp;
using POO2_Ex9.Observers;

namespace POO2_Ex9.Server;

public class SmtpServer : IObservable<MailMessage>
{
    private readonly List<IObserver<MailMessage>> _observers;
    private readonly int _port;

    private SimpleSmtpServer? _server;

    public SmtpServer(int port)
    {
        _port = port;
        _observers = new List<IObserver<MailMessage>>();
    }

    public IDisposable Subscribe(IObserver<MailMessage> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new MailUnsubscriber(_observers, observer);
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
            foreach (IObserver<MailMessage> observer in _observers)
                observer.OnNext(mailMessage);
        };
    }
}