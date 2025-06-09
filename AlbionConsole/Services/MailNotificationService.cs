using System.Net;
using System.Net.Mail;
using System.Text.Json;
using AlbionConsole.Models;

namespace AlbionConsole.Services;

public class MailNotificationService
{
    private readonly EmailConfig _emailConfig;

    public MailNotificationService()
    {
        _emailConfig = LoadEmailConfig();
    }

    private EmailConfig LoadEmailConfig()
    {
        var configPath = Path.Combine("Data", "mailconfig.json");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Email configuration file not found at: {configPath}");
        }

        string json = File.ReadAllText(configPath);
        Console.WriteLine("mailconfig loaded successfully");
        EmailConfig? config = JsonSerializer.Deserialize<EmailConfig>(json);

        return config ?? throw new InvalidOperationException("Failed to deserialize mailconfig.json");
    }

    public async Task SendNotificationAsync(string toEmail, string subject, string body)
    {
        var message = new MailMessage()
        {
            From = new MailAddress(_emailConfig.SenderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(toEmail));

        using var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port)
        {
            Port = _emailConfig.Port,
            Credentials = new NetworkCredential(_emailConfig.SenderEmail, _emailConfig.SenderPassword),
            EnableSsl = _emailConfig.EnableSsl
        };
        
        await client.SendMailAsync(message);
    }
} 