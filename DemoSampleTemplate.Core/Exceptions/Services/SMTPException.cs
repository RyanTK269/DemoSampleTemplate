using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.Services
{
    public class SMTPException : BaseServiceException
    {
        public SMTPException() : base() { }

        public void ThrowSmtpCommandException(SmtpCommandException ex)
        {
            throw new Exception($"Error trying to connect: {ex.Message} - StatusCode: {ex.StatusCode}");
        }

        public void ThrowSmtpProtocolException(SmtpProtocolException ex)
        {
            throw new Exception($"Protocol error while trying to connect: {ex.Message}");
        }

        public void ThrowAuthenticationException(AuthenticationException ex)
        {
            throw new Exception($"Authentication failed - Invalid user name or password");
        }
    }
}
