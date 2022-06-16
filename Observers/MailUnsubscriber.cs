using System.Net.Mail;

namespace POO2_Ex9.Observers;

public sealed class MailUnsubscriber : IDisposable
{
    private readonly IObserver<MailMessage> _observer;
    private readonly List<IObserver<MailMessage>> _observers;

    public MailUnsubscriber(List<IObserver<MailMessage>> observers, IObserver<MailMessage> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    public void Dispose()
    {
        if (_observers.Contains(_observer))
            _observers.Remove(_observer);
    }
}