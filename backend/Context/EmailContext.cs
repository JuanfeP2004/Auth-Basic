using System.Net;
using System.Net.Mail;

public class EmailContext {
    
    private readonly IConfiguration _configuration;
    private readonly SmtpClient smtpClient;

    string? host, user, password;
    int port;
    public EmailContext(IConfiguration configuration)
    {
        _configuration = configuration;
        smtpClient = new SmtpClient();

        host = _configuration["SmtpSettings:Server"];
        port = Convert.ToInt32(_configuration["SmtpSettings:Port"]);
        user = _configuration["SmtpSettings:Username"];
        password = _configuration["SmtpSettings:Password"];

        smtpClient.Host = host;
        smtpClient.Port = port;
        smtpClient.Credentials = new NetworkCredential(user, password);
        smtpClient.EnableSsl = true;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            MailMessage message = new MailMessage(user, to, subject, body);
            await smtpClient.SendMailAsync(message);
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.ToString());
            return false;
        }
    }
}