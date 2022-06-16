using System.Net.Mail;
using System.Text;

namespace POO2_Ex9.Observers;

public class MailStatObserver : IMailObserver
{
    private readonly string _fileName;

    private int _receivedCount;
    private int _rejectedBytes;
    private int _rejectedCount;
    private int _storedBytes;

    public MailStatObserver(string fileName)
    {
        _fileName = fileName;
    }

    public void OnReceive(MailMessage mail)
    {
        _receivedCount++;
    }

    public void OnStore(MailMessage mail)
    {
        _storedBytes += Encoding.Default.GetByteCount(mail.Body);

        WriteStatsInFile();
    }

    public void OnReject(MailMessage mail)
    {
        _rejectedCount++;
        _rejectedBytes += Encoding.Default.GetByteCount(mail.Body);

        WriteStatsInFile();
    }

    private void WriteStatsInFile()
    {
        using var file = new StreamWriter(_fileName);
        file.WriteLine($"Received count  : {_receivedCount}");
        file.WriteLine($"Rejected count  : {_rejectedCount}");
        file.WriteLine($"Stored bytes    : {_storedBytes}");
        file.WriteLine($"Rejected bytes  : {_rejectedBytes}");
        file.WriteLine($"Spam ratio      : {_rejectedCount * 100 / _receivedCount}%");
        file.WriteLine($"Spam bytes rate : {(float)_rejectedBytes / _storedBytes:0.00}");
    }
}