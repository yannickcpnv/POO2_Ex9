using POO2_Ex9.Observers;
using POO2_Ex9.Server;
using POO2_Ex9.Validation;

namespace POO2_Ex9;

internal static class Program
{
    private static void Main()
    {
        var exitEvent = new ManualResetEvent(false);
        WaitForExitInput(exitEvent);

        var smtpSever = new SmtpServer(3325, new MailValidator());
        smtpSever.Subscribe(new MailStatObserver());
        smtpSever.Subscribe(new MailLogObserver());
        smtpSever.Start();
        smtpSever.WaitReceivingMessages();

        exitEvent.WaitOne();

        smtpSever.Stop();
    }

    private static void WaitForExitInput(EventWaitHandle exitEvent)
    {
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            exitEvent.Set();
        };
    }
}