using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pacer.Data.Services;

namespace Pacer.Data.Services;

public class SmtpMailService : IMailService
{
    protected readonly ILogger _logger;
    private readonly string _from;
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;

    // appsettings.json section MailSettings contains mail configuration
    public SmtpMailService(IConfiguration config, ILogger<SmtpMailService> logger)
{
    _logger = logger;
    // First, try to get values from environment variables
    _from = Environment.GetEnvironmentVariable("FromAddress");
    _host = Environment.GetEnvironmentVariable("Host");
    _port = Convert.ToInt32(Environment.GetEnvironmentVariable("Port"));
    _username = Environment.GetEnvironmentVariable("UserName");
    _password = Environment.GetEnvironmentVariable("Password");

    // If environment variables aren't set (i.e., we're in a local environment), 
    // fall back to using appsettings.json
    if (string.IsNullOrEmpty(_from))
    {
        _from = config.GetSection("MailSettings")["FromAddress"] ?? string.Empty;
    }
    if (string.IsNullOrEmpty(_host))
    {
        _host = config.GetSection("MailSettings")["Host"] ?? string.Empty;
    }
    if (_port == 0) // assuming port can't be 0
    {
        _port = Int32.Parse(config.GetSection("MailSettings")["Port"] ?? "0");
    }
    if (string.IsNullOrEmpty(_username))
    {
        _username = config.GetSection("MailSettings")["UserName"] ?? string.Empty;
    }
    if (string.IsNullOrEmpty(_password))
    {
        _password = config.GetSection("MailSettings")["Password"] ?? string.Empty;
    }
}

    // send mail
    public bool SendMail(string subject, string body, string to, string from = null, bool asHtml = true)
    {
        // now configure smtp client
        var client = new SmtpClient(_host, _port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 20000
        };
        _logger.LogInformation($"Sending email from {_from} to {to}. {client.Host}:{client.Port}");
        try
        {
            // construct the mail message
            var mail = new MailMessage
            {
                From = new MailAddress(from ?? _from),
                Subject = subject,
                Body = body,
                IsBodyHtml = asHtml,

            };
            mail.To.Add(to);

            // now send the mail message
            try
            {
                client.Send(mail);
                _logger.LogInformation($"Email sent to {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error constructing email: {ex.Message}");
            return false;
        }
    }

    // Send Mail Asynchronously
    // public async Task<bool> SendMailAsync(string subject, string body, string to, string from = null, bool asHtml = true)
    // {
    //     // now configure smtp client 
    //     var client = new SmtpClient(_host, _port)
    //     {
    //         Credentials = new NetworkCredential(_username, _password),
    //         EnableSsl = true
    //     };
    //     try
    //     {
    //         // construct the mail message
    //         var mail = new MailMessage
    //         {
    //             From = new MailAddress(from ?? _from),
    //             Subject = subject,
    //             Body = body,
    //             IsBodyHtml = asHtml,
    //         };

    //         mail.To.Add(to);

    //         try
    //         {
    //             // ... (rest of the method)
    //             client.Send(mail);
    //             return true;
    //         }
    //         catch (Exception ex) // Catch the exception
    //         {
    //             // Log the exception message to the console
    //             Console.WriteLine($"Error sending email: {ex.Message}");
    //             return false;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error constructing email: {ex.Message}");
    //         return false;
    //     }
    // }
}

