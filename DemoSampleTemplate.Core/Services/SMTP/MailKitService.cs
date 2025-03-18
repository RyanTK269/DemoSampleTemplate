using DemoSampleTemplate.Core.DataObjects.Config;
using DemoSampleTemplate.Core.DataObjects.Mail;
using DemoSampleTemplate.Core.Exceptions.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DemoSampleTemplate.Core.Services.SMTP
{
    public class MailKitService
    {
        private readonly SMTPConfig _smtpConfig;
        private SMTPException _smtpException;

        private const string DEFAULT_SMTP_SERVER = "";
        private const int DEFAULT_SMTP_PORT = 587;
        private const string DEFAULT_EMAIL_SENDER = "";

        public MailKitService(SMTPConfig configData)
        {
            _smtpConfig = configData;
            _smtpException = new SMTPException();
        }

        public SMTPResponse SendMail(MailItem mailItem)
        {
            using (MimeMessage message = new MimeMessage())
            {
                try
                {
                    GenerateMailRequest(message, mailItem);
                    SendMessage(message).Wait();
                    return new SMTPResponse
                    {
                        Success = true,
                        Result = mailItem.Entity
                    };
                }
                catch (Exception ex)
                {
                    return SMTPResponse.GetErrorResponse($"An error occured when sending an email. Error: " + ex.Message.ToString(), mailItem.Entity, ex);
                }
            }
        }

        public async Task<SMTPResponse> SendMailAsync(MailItem mailItem)
        {
            using (MimeMessage message = new MimeMessage())
            {
                try
                {
                    GenerateMailRequest(message, mailItem);
                    await SendMessage(message);
                    return new SMTPResponse
                    {
                        Success = true,
                        Result = mailItem.Entity
                    };
                }
                catch (Exception ex)
                {
                    return SMTPResponse.GetErrorResponse($"An error occured when sending an email. Error: " + ex.Message.ToString(), mailItem.Entity, ex);
                }
                finally
                {
                    message.Dispose();
                }
            }
        }

        private async Task SendMessage(MimeMessage message)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = MySslCertificateValidationCallback;
                    await client.ConnectAsync(_smtpConfig.SmtpServer ?? DEFAULT_SMTP_SERVER, _smtpConfig.SmtpPort > 0 ? _smtpConfig.SmtpPort : DEFAULT_SMTP_PORT, SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(_smtpConfig.SmtpUsername, _smtpConfig.SmtpPassword);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                _smtpException.ThrowException(ex);
            }
        }

        private void GenerateMailRequest(MimeMessage mailMessage, MailItem mailItem)
        {
            mailMessage.Subject = mailItem.Subject;
            CreateMailSenderAddress(mailMessage, mailItem.SenderDisplayName, mailItem.From);
            CreateMailReceiverAddress(mailMessage.To, mailItem.To);
            CreateMailReceiverAddress(mailMessage.Cc, mailItem.GroupCC);
            CreateMailReceiverAddress(mailMessage.Bcc, mailItem.GroupBCC);
            CreateMailBody(mailMessage, mailItem.Body, mailItem.EmailAttachments);
        }


        private void CreateMailSenderAddress(MimeMessage mailMessage, string senderDisplayFrom, string mailAddressFrom)
        {
            if (string.IsNullOrWhiteSpace(mailAddressFrom))
            {
                mailAddressFrom = DEFAULT_EMAIL_SENDER;
            }
            mailMessage.From.Add(new MailboxAddress(senderDisplayFrom, mailAddressFrom));
        }

        private void CreateMailReceiverAddress(InternetAddressList listAddrress, IEnumerable<MailAddress> listMailTo)
        {
            listMailTo.ToList().ForEach(mail => listAddrress.Add(new MailboxAddress(mail.DisplayName, mail.EmailAddress)));
        }

        private void CreateMailBody(MimeMessage mailMessage, string body, IEnumerable<MailAttachment> attachments)
        {
            var builder = new BodyBuilder();
            attachments.ToList().ForEach(attachment =>
            {
                builder.Attachments.Add(attachment.FileName, attachment.Bytes);
            });
            builder.HtmlBody = body;

            mailMessage.Body = builder.ToMessageBody();
        }

        private bool MySslCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If there are no errors, then everything went smoothly.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Note: MailKit will always pass the host name string as the `sender` argument.
            var host = (string)sender;

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
            {
                // This means that the remote certificate is unavailable. Notify the user and return false.
                //_logger.LogError(string.Format("The SSL certificate was not available for {0}", host));
                return false;
            }

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                // This means that the server's SSL certificate did not match the host name that we are trying to connect to.
                var certificate2 = certificate as X509Certificate2;
                var cn = certificate2 != null ? certificate2.GetNameInfo(X509NameType.SimpleName, false) : certificate.Subject;

                //_logger.LogError(string.Format("The Common Name for the SSL certificate did not match {0}. Instead, it was {1}.", host, cn));
                return false;
            }

            // The only other errors left are chain errors.
            string errorSSLMessage = "The SSL certificate for the server could not be validated for the following reasons:";

            // The first element's certificate will be the server's SSL certificate (and will match the `certificate` argument)
            // while the last element in the chain will typically either be the Root Certificate Authority's certificate -or- it
            // will be a non-authoritative self-signed certificate that the server admin created. 
            foreach (var element in chain.ChainElements)
            {
                // Each element in the chain will have its own status list. If the status list is empty, it means that the
                // certificate itself did not contain any errors.
                if (element.ChainElementStatus.Length == 0)
                    continue;

                errorSSLMessage = string.Concat(errorSSLMessage, string.Format("\u2022 {0}", element.Certificate.Subject));
                foreach (var error in element.ChainElementStatus)
                {
                    // `error.StatusInformation` contains a human-readable error string while `error.Status` is the corresponding enum value.
                    errorSSLMessage = string.Concat(errorSSLMessage, string.Format("\t\u2022 {0}", error.StatusInformation));
                }
            }
            return false;
        }
    }
}
