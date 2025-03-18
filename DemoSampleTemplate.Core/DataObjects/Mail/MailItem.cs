using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.Mail
{
    /// <summary>
    /// Defines the components for sending mail
    /// </summary>
    public class MailItem
    {
        /// <summary>
        /// Stores the mail subject 
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Stores the mail body 
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Stores the sender address 
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Stores the displayed name for the sender 
        /// </summary>
        public string SenderDisplayName { get; set; }
        /// <summary>
        /// Stores the list of recipients 
        /// </summary>
        public IEnumerable<MailAddress> To { get; set; } = new List<MailAddress>();
        /// <summary>
        /// Stores the list of CC recipients 
        /// </summary>
        public IEnumerable<MailAddress> GroupCC { get; set; } = new List<MailAddress>();
        /// <summary>
        /// Stores the list of BCC recipients 
        /// </summary>
        public IEnumerable<MailAddress> GroupBCC { get; set; } = new List<MailAddress>();
        /// <summary>
        /// Stores the list of Reply recipients 
        /// </summary>
        public IEnumerable<MailAddress> GroupReplyTo { get; set; } = new List<MailAddress>();
        /// <summary>
        /// Stores the list of attachments in mail 
        /// </summary>
        public IEnumerable<MailAttachment> EmailAttachments { get; set; } = new List<MailAttachment>();
        /// <summary>
        /// Stores the object related in mail 
        /// </summary>
        public object Entity { get; set; }
        /// <summary>
        /// Stores the identification for tracing mail 
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// Stores the list of tags in email. Separated with ';' 
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// Stores the StreamIdentifier 
        /// </summary>
        public string StreamIdentifier { get; set; }
        /// <summary>
        /// Stores the ServiceIdentifier 
        /// </summary>
        public string ServiceIdentifier { get; set; }
    }
}
