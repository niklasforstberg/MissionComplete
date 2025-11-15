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

            // Try colon format first, fallback to double underscore
            var server = _configuration["smtp:server"] ?? _configuration["smtp__server"];
            var port = _configuration["smtp:sslport"] ?? _configuration["smtp__sslport"];
            var username = _configuration["smtp:username"] ?? _configuration["smtp__username"];
            var password = _configuration["smtp:password"] ?? _configuration["smtp__password"];
            var enableSsl = _configuration["smtp:enablessl"] ?? _configuration["smtp__enablessl"];

            var smtpClient = new SmtpClient(server ?? throw new InvalidOperationException("SMTP server not configured"))
            {
                Port = int.Parse(port ?? "2525"),
                Credentials = new NetworkCredential(username ?? throw new InvalidOperationException("SMTP username not configured"), 
                    password ?? throw new InvalidOperationException("SMTP password not configured")),
                EnableSsl = bool.Parse(enableSsl ?? "false"),
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
    <a href='http://localhost:8085/welcome/?token={invitation.Token}' class='button'>Accept Invitation</a>
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

        public async Task SendPasswordResetEmail(string email, string token)
        {
            _logger.LogDebug("Starting to send password reset email to {Email}", email);

            // Try colon format first, fallback to double underscore
            var server = _configuration["smtp:server"] ?? _configuration["smtp__server"];
            var port = _configuration["smtp:sslport"] ?? _configuration["smtp__sslport"];
            var username = _configuration["smtp:username"] ?? _configuration["smtp__username"];
            var password = _configuration["smtp:password"] ?? _configuration["smtp__password"];
            var enableSsl = _configuration["smtp:enablessl"] ?? _configuration["smtp__enablessl"];


            var smtpClient = new SmtpClient(server ?? throw new InvalidOperationException("SMTP server not configured"))
            {
                Port = int.Parse(port ?? "2525"),
                Credentials = new NetworkCredential(username ?? throw new InvalidOperationException("SMTP username not configured"), 
                    password ?? throw new InvalidOperationException("SMTP password not configured")),
                EnableSsl = bool.Parse(enableSsl ?? "false"),
            };

            _logger.LogDebug("SMTP client configured with host: {SmtpHost}, port: {SmtpPort}, SSL: {EnableSsl}",
                smtpClient.Host, smtpClient.Port, smtpClient.EnableSsl);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("coach@forstberg.com", "MissionComplete"),
                Subject = "Reset Your Password - MissionComplete",
                Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
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
    <h1>Password Reset Request</h1>
    <p>Hi there!</p>
    <p>We received a request to reset your password for your MissionComplete account.</p>
    <p>Click the button below to reset your password:</p>
    <a href='http://localhost:8085/reset-password?token={token}' class='button'>Reset Password</a>
    <p>This link will expire in 24 hours.</p>
    <p>If you didn't request a password reset, please ignore this email.</p>
    <p>Best regards,<br>The MissionComplete Team</p>
</body>
</html>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            _logger.LogDebug("Mail message created with subject: {Subject}", mailMessage.Subject);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Password reset email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }

        public async Task SendVerificationEmail(string email, string token)
        {
            _logger.LogDebug("Starting to send verification email to {Email}", email);

            // Try colon format first, fallback to double underscore
            var server = _configuration["smtp:server"] ?? _configuration["smtp__server"];
            var port = _configuration["smtp:sslport"] ?? _configuration["smtp__sslport"];
            var username = _configuration["smtp:username"] ?? _configuration["smtp__username"];
            var password = _configuration["smtp:password"] ?? _configuration["smtp__password"];
            var enableSsl = _configuration["smtp:enablessl"] ?? _configuration["smtp__enablessl"];
            var baseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:8085";

            var smtpClient = new SmtpClient(server ?? throw new InvalidOperationException("SMTP server not configured"))
            {
                Port = int.Parse(port ?? "2525"),
                Credentials = new NetworkCredential(username ?? throw new InvalidOperationException("SMTP username not configured"), 
                    password ?? throw new InvalidOperationException("SMTP password not configured")),
                EnableSsl = bool.Parse(enableSsl ?? "false"),
            };

            _logger.LogDebug("SMTP client configured with host: {SmtpHost}, port: {SmtpPort}, SSL: {EnableSsl}",
                smtpClient.Host, smtpClient.Port, smtpClient.EnableSsl);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("coach@forstberg.com", "MissionComplete"),
                Subject = "Verify Your Email - MissionComplete",
                Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Verification</title>
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
    <h1>Verify Your Email Address</h1>
    <p>Hi there!</p>
    <p>Thank you for registering with MissionComplete. Please verify your email address by clicking the button below:</p>
    <a href='{baseUrl}/verify-email?token={token}' class='button'>Verify Email</a>
    <p>This link will expire in 24 hours.</p>
    <p>If you didn't create an account, please ignore this email.</p>
    <p>Best regards,<br>The MissionComplete Team</p>
</body>
</html>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            _logger.LogDebug("Mail message created with subject: {Subject}", mailMessage.Subject);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Verification email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to {Email}", email);
                throw;
            }
        }
    }
}
