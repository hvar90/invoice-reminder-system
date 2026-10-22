using MailKit.Net.Smtp;
using MimeKit;

namespace FacturasApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _config.GetSection("EmailSettings");
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

         
       /*  
        Le indica a MailKit que no falle si la lista de revocación no responde/no se puede comprobar
        es una práctica estándar en entornos donde la infraestructura de red 
        o proveedores en la nube (AWS, Azure, Docker, Heroku) restringen las consultas 
        de revocación salientes. El certificado SSL del servidor SMTP de Brevo se seguirá auditando 
        y validando normalmente contra las Entidades Certificadoras (CA) raíz de la máquina. */
        client.CheckCertificateRevocation = false;

        await client.ConnectAsync(
            emailSettings["SmtpServer"], 
            int.Parse(emailSettings["Port"]!), 
            MailKit.Security.SecureSocketOptions.StartTls
        );
        await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}


