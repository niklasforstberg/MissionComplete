using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MissionComplete.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MissionComplete.Integrations
{
    public class SmtpEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendInvitationEmail(InvitationDto invitation)
        {
            _logger.LogDebug("Starting to send invitation email to {InviteeEmail}", invitation.InviteeEmail);

            var smtpClient = new SmtpClient(_configuration["SmtpServer"])
            {
                Port = int.Parse(_configuration["SmtpPort"] ?? "2525"),
                Credentials = new NetworkCredential(_configuration["SmtpUsername"], _configuration["SmtpPassword"]),
                EnableSsl = bool.Parse(_configuration["SmtpEnableSsl"] ?? "false"),
            };

            _logger.LogDebug("SMTP client configured with host: {SmtpHost}, port: {SmtpPort}, SSL: {EnableSsl}",
                smtpClient.Host, smtpClient.Port, smtpClient.EnableSsl);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("coach@forstberg.com", "MissionComplete"),
                Subject = "You're invited to join a team in the MissionComplete app!",
                Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Chores App Invitation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        h1 {{
            color: #4a4a4a;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #ffffff;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <h1>Welcome to the Mission Complete App!</h1>
    <p>Hi there!</p>
    <p>You've been invited by {invitation.InviterName} to join {invitation.TeamName}.</p>
    <p>Click the button below to accept the invitation and get started:</p>
    <a href='http://localhost:8084/accept-invitation/{invitation.Token}/accept' class='button'>Accept Invitation</a>
    <p>If you have any questions, please don't hesitate to contact {invitation.InviterName}.</p>
    <p>Best regards,<br>The MissionComplete Team</p>
</body>
</html>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(invitation.InviteeEmail);

            _logger.LogDebug("Mail message created with subject: {Subject}", mailMessage.Subject);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Invitation email sent successfully to {InviteeEmail}", invitation.InviteeEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invitation email to {InviteeEmail}", invitation.InviteeEmail);
                throw;
            }
        }
    }
}
