using System.Net.Mail;

namespace POO2_Ex9.Observers;

public interface IMailObserver
{
    public void OnReceive(MailMessage mail);
    public void OnStore(MailMessage mail);
    public void OnReject(MailMessage mail);
}