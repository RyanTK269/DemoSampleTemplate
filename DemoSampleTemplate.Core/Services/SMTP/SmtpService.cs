using System.Net;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DemoSampleTemplate.Core.DataObjects.Config;
using DemoSampleTemplate.Core.Exceptions.Services;
using DemoSampleTemplate.Core.DataObjects.Mail;

namespace DemoSampleTemplate.Core.Services.SMTP
{
    /// <summary>
    /// Defines class SmtpService for sending email
    /// </summary>
    /// <seealso cref="ISmtpService" />
    public class SmtpService
    {
        /// <summary>
        /// The base configuration for Smtp
        /// </summary>
        private SMTPConfig _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpService<T>"/> class.
        /// Config sample
        /// var service = new SmtpService(SmtpConfiguration configuration, ILogger logger)
        /// {
        ///    _configuration = configuration,
        ///    _logger = logger,
        /// };  
        /// </summary>
        /// <param name="configuration">The NotifyApiConfiguration.</param>
        /// <param name="logger">The Version of API to process request.</param>
        public SmtpService(SMTPConfig configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send asynchronously email
        /// </summary>
        /// <param name="mailItem">The structure of email</param>
        /// <returns></returns>
        public async Task<SMTPResponse> SendMailAsync(MailItem mailItem)
        {
            using (MailMessage message = new MailMessage())
            {
                try
                {
                    GenerateMailRequest(message, mailItem);
                    await SendMessage(message);
                    return SMTPResponse.GetSuccessResponse(mailItem.Entity);
                }
                catch (Exception ex)
                {
                    var errorMessages = new List<string>() { $"An error occured when sending an email - {mailItem.ProcessId}: " + ex.Message.ToString() };
                    return SMTPResponse.GetErrorResponse(errorMessages, mailItem.Entity, ex);
                }
                finally
                {
                    message.Dispose();
                }
            }
        }

        /// <summary>
        /// Send synchronously email
        /// </summary>
        /// <param name="mailItem">The structure of email</param>
        /// <returns></returns>
        public SMTPResponse SendMail(MailItem mailItem)
        {
            using (MailMessage message = new MailMessage())
            {
                try
                {
                    GenerateMailRequest(message, mailItem);
                    SendMessage(message).Wait();
                    return SMTPResponse.GetSuccessResponse(mailItem.Entity);
                }
                catch (Exception ex)
                {
                    var errorMessages = new List<string>() { $"An error occured when sending an email - {mailItem.ProcessId}: " + ex.Message.ToString() };
                    return SMTPResponse.GetErrorResponse(errorMessages, mailItem.Entity, ex);
                }
                finally
                {
                    message.Dispose();
                }
            }
        }

        /// <summary>
        /// Send message to Smtp server
        /// </summary>
        /// <param name="message">The structure of mail</param>
        /// <returns></returns>
        private async Task SendMessage(MailMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Port = _configuration.SmtpPort;
                client.Host = _configuration.SmtpServer;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_configuration.SmtpUsername, _configuration.SmtpPassword);
                await client.SendMailAsync(message);
            }
        }

        /// <summary>
        /// Generate the structure for message
        /// </summary>
        /// <param name="mailMessage">The structure of the mail</param>
        /// <param name="mailItem">The component in the mail</param>
        /// <returns></returns>
        private void GenerateMailRequest(MailMessage mailMessage, MailItem mailItem)
        {
            mailMessage.Subject = mailItem.Subject;
            CreateMailSenderAddress(mailMessage, _configuration.Sender, _configuration.Sender);
            CreateMailReceiverAddress(mailMessage.To, mailItem.To);
            CreateMailReceiverAddress(mailMessage.CC, mailItem.GroupCC);
            CreateMailReceiverAddress(mailMessage.Bcc, mailItem.GroupBCC);
            CreateMailReceiverAddress(mailMessage.ReplyToList, mailItem.GroupReplyTo);
            CreateMailBody(mailMessage, mailItem.Body, mailItem.EmailAttachments);
        }

        /// <summary>
        /// Generate the message Sender
        /// </summary>
        /// <param name="mailMessage">The structure of the mail</param>
        /// <param name="senderDisplayFrom">The sender displaying name</param>
        /// <param name="mailAddressFrom">The sender email</param>
        /// <returns></returns>
        private void CreateMailSenderAddress(MailMessage mailMessage, string senderDisplayFrom, string mailAddressFrom)
        {
            mailMessage.From = new System.Net.Mail.MailAddress(senderDisplayFrom, mailAddressFrom);
            mailMessage.Sender = new System.Net.Mail.MailAddress(senderDisplayFrom, mailAddressFrom);
        }

        /// <summary>
        /// Generate the message recipients
        /// </summary>
        /// <param name="listAddrress">The list of recipients</param>
        /// <param name="listMailTo">The list of mail sent</param>
        /// <returns></returns>
        private void CreateMailReceiverAddress(MailAddressCollection listAddrress, IEnumerable<DataObjects.Mail.MailAddress> listMailTo)
        {
            listMailTo.ToList().ForEach(mail => listAddrress.Add(new System.Net.Mail.MailAddress(mail.EmailAddress)));
        }

        /// <summary>
        /// Generate the message body
        /// </summary>
        /// <param name="mailMessage">The structure of the mail</param>
        /// <param name="body">The message body</param>
        /// <param name="attachments">The list of attachments</param>
        /// <returns></returns>
        private void CreateMailBody(MailMessage mailMessage, string body, IEnumerable<MailAttachment> attachments)
        {
            attachments.ToList().ForEach(attachment =>
            {
                Stream streamAtt = new MemoryStream(attachment.Bytes);
                mailMessage.Attachments.Add(new Attachment(streamAtt, attachment.FileName));
            });
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
        }
    }
}
