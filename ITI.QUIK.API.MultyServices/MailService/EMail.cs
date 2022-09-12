using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models.EMail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace MailService
{
    public class EMail : IEMail
    {
        private ILogger<EMail> _logger;
        private SMTPMailConfig _connections;

        public EMail(ILogger<EMail> logger, IOptions<SMTPMailConfig> connections)
        {
            _logger=logger;
            _connections = connections.Value;
        }

        public async Task Send(NewEMail email)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} EMail Send Called, Subj={email.Subject}");

            string Host = System.Net.Dns.GetHostName();

            using (MailMessage message = new MailMessage
            {
                To =
                {
                    new MailAddress(
                        _connections.MainReciever,
                        _connections.MainReciever)
                },
                Sender = new MailAddress(_connections.SenderEmail, _connections.SenderEmailAlias),
                From = new MailAddress(_connections.SenderEmail, _connections.SenderEmailAlias),
                Subject = $"{Host} ({_connections.ServerType}) {email.Subject}" ,
                Body = email.Body,
                IsBodyHtml = true
            })
            {
                using (
                   SmtpClient smtp = new SmtpClient
                   {
                       Host = _connections.SMTPHost,
                       Port = _connections.SMTPPort                     
                   }
                )
                {
                    smtp.Credentials = new System.Net.NetworkCredential(_connections.Login, _connections.Password);

                    if (_connections.CCReceiversEmail is not null)
                    {
                        foreach (string reciever in _connections.CCReceiversEmail)
                        {
                            MailAddress copy = new MailAddress(reciever);
                            message.CC.Add(copy);
                        }
                    }

                    try { await smtp.SendMailAsync(message); }
                    catch (Exception excp)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} EMail Send Exception, {excp.Message}");
                    }
                }
            }
        }
    }
}