using FidelityCard.Application.Interfaces;
using FidelityCard.Lib.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FidelityCard.Srv.Services;

/// <summary>
/// Implementazione del servizio di invio email
/// </summary>
public class EmailService : Application.Interfaces.IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> InviaEmailVerificaAsync(
        string email, 
        string nome, 
        string token, 
        string linkRegistrazione, 
        string? puntoVenditaNome)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName ?? "Fidelity Card", _emailSettings.Sender));
            message.To.Add(new MailboxAddress(nome, email));
            message.Subject = "🎁 Completa la tua registrazione Fidelity Card";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildVerificationEmailBody(nome, puntoVenditaNome ?? "il nostro punto vendita", linkRegistrazione, token)
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore invio email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InviaEmailBenvenutoAsync(
        string email, 
        string nome, 
        string codiceFidelity, 
        byte[]? cardDigitale = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName ?? "Fidelity Card", _emailSettings.Sender));
            message.To.Add(new MailboxAddress(nome, email));
            message.Subject = $"🎉 Benvenuto {nome}! La tua Fidelity Card è pronta";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildWelcomeEmailBody(nome, codiceFidelity)
            };

            // Allega la card digitale
            if (cardDigitale != null && cardDigitale.Length > 0)
            {
                bodyBuilder.Attachments.Add($"SunsFidelityCard_{codiceFidelity}.png", cardDigitale, ContentType.Parse("image/png"));
            }

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore invio email benvenuto: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InviaEmailAccessoProfiloAsync(
        string email, 
        string nome, 
        string linkAccesso)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName ?? "Fidelity Card", _emailSettings.Sender));
            message.To.Add(new MailboxAddress(nome, email));
            message.Subject = "🔑 Accedi alla tua area personale Fidelity Card";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildProfileAccessEmailBody(nome, linkAccesso)
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore invio email accesso: {ex.Message}");
            return false;
        }
    }

    private async Task SendEmailAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        
        await client.ConnectAsync(
            _emailSettings.MailServer,
            _emailSettings.MailPort,
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _emailSettings.Sender,
            _emailSettings.Password
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static string BuildVerificationEmailBody(string nome, string puntoVendita, string link, string token)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 0 20px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); color: white; padding: 40px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 40px 30px; }}
        .content h2 {{ color: #333; margin-top: 0; }}
        .content p {{ color: #666; line-height: 1.6; font-size: 16px; }}
        .info-box {{ background-color: #f8f9fa; border-left: 4px solid #105a12ff; padding: 15px; margin: 20px 0; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #999; font-size: 14px; }}
        .token {{ font-size: 24px; font-weight: bold; color: #105a12ff; letter-spacing: 2px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>☀️ Suns - Zero&Company</h1>
            <p style='margin: 10px 0 0 0;'>La tua Fidelity Card ti aspetta!</p>
        </div>
        <div class='content'>
            <h2>Ciao! 👋</h2>
            <p>Per completare la tua registrazione e ricevere la tua Suns Fidelity Card digitale, clicca sul pulsante qui sotto:</p>
            
            <div style='text-align: center; margin: 20px 0;'>
                <table border='0' cellpadding='0' cellspacing='0' style='margin: 0 auto;'>
                    <tr>
                        <td align='center' bgcolor='#105a12' style='background-color: #105a12; background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); border-radius: 5px; padding: 15px 40px;'>
                            <a href='{link}' style='color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; text-decoration: none; display: inline-block;'>
                                <span style='color: #ffffff; text-decoration: none;'>COMPLETA REGISTRAZIONE</span>
                            </a>
                        </td>
                    </tr>
                </table>
            </div>

            <div class='info-box'>
                <p style='margin: 0;'><strong>⏰ Attenzione:</strong> Questo link è valido per <strong>15 minuti</strong>.</p>
                <p style='margin: 10px 0 0 0;'>Se non riesci a cliccare il pulsante, copia questo link nel tuo browser:</p>
                <p style='word-break: break-all; color: #105a12ff; margin: 10px 0 0 0;'>{link}</p>
            </div>
        </div>
        <div class='footer'>
            <p>© 2024 Suns - Zero&Company. Tutti i diritti riservati.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildWelcomeEmailBody(string nome, string codiceFidelity)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 0 20px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); color: white; padding: 40px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 40px 30px; }}
        .content h2 {{ color: #333; margin-top: 0; }}
        .content p {{ color: #666; line-height: 1.6; font-size: 16px; }}
        .code-box {{ background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); color: white; padding: 30px; text-align: center; border-radius: 10px; margin: 30px 0; }}
        .code-box h2 {{ margin: 0 0 10px 0; font-size: 18px; }}
        .code {{ font-size: 36px; font-weight: bold; letter-spacing: 3px; }}
        .benefits {{ background-color: #f8f9fa; padding: 20px; border-radius: 10px; margin: 20px 0; }}
        .benefits li {{ margin: 10px 0; color: #666; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #999; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>☀️ Benvenuto in Suns!</h1>
            <p style='margin: 10px 0 0 0;'>La tua Fidelity Card è attiva</p>
        </div>
        <div class='content'>
            <h2>Ciao {nome}! 🎉</h2>
            <p>La tua registrazione è stata completata con successo!</p>
            
            <div class='code-box'>
                <h2>Il tuo Codice Fidelity</h2>
                <div class='code'>{codiceFidelity}</div>
            </div>

            <p><strong>📱 La tua card digitale è allegata a questa email.</strong></p>

            <div class='benefits'>
                <h3 style='color: #333; margin-top: 0;'>✨ I tuoi vantaggi:</h3>
                <ul>
                    <li><strong>Accumula punti</strong> ad ogni acquisto</li>
                    <li><strong>Sconti esclusivi</strong> riservati ai membri</li>
                    <li><strong>Promozioni speciali</strong> in anteprima</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>© 2024 Suns - Zero&Company. Tutti i diritti riservati.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string BuildProfileAccessEmailBody(string nome, string linkAccesso)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 0 20px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); color: white; padding: 40px 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 40px 30px; }}
        .content h2 {{ color: #333; margin-top: 0; }}
        .content p {{ color: #666; line-height: 1.6; font-size: 16px; }}
        .info-box {{ background-color: #e9ecef; border-left: 4px solid #105a12ff; padding: 15px; margin: 20px 0; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #999; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>☀️ Fidelity Card</h1>
            <p style='margin: 10px 0 0 0;'>Bentornato!</p>
        </div>
        <div class='content'>
            <h2>Ciao {nome}! 👋</h2>
            <p>Abbiamo ricevuto una richiesta di accesso alla tua area personale.</p>
            
            <div style='text-align: center; margin: 20px 0;'>
                <table border='0' cellpadding='0' cellspacing='0' style='margin: 0 auto;'>
                    <tr>
                        <td align='center' bgcolor='#105a12' style='background-color: #105a12; background: linear-gradient(135deg, #105a12ff 0%, #053e30ff 100%); border-radius: 5px; padding: 15px 40px;'>
                            <a href='{linkAccesso}' style='color: #ffffff; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; text-decoration: none; display: inline-block;'>
                                <span style='color: #ffffff; text-decoration: none;'>ACCEDI AL PROFILO</span>
                            </a>
                        </td>
                    </tr>
                </table>
            </div>

            <div class='info-box'>
                <p style='margin: 0;'><strong>⏰ Link valido per 15 minuti.</strong></p>
                <p style='margin: 10px 0 0 0;'>Se non hai richiesto tu l'accesso, puoi ignorare questa email.</p>
            </div>
        </div>
        <div class='footer'>
            <p>© 2024 Suns - Zero&Company. Tutti i diritti riservati.</p>
        </div>
    </div>
</body>
</html>";
    }
}
