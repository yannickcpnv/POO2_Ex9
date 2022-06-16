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

        var mailValidator = new MailValidator();
        string[] wantedValidators = { "BadWords", "WhiteListRecipients", "AttachmentsExtension" };
        wantedValidators.Select(MailValidatorFactory.Create).ToList()
                        .ForEach(mailValidator.Add);

        var smtpSever = new SmtpServer(3325, mailValidator);
        smtpSever.Subscribe(new MailStatObserver("data/stats.txt"));
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