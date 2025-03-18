using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.Config
{
    public class SMTPConfig
    {
        /// <summary>
        /// Stores the Smtp server address 
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// Stores the Smtp server port 
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// Stores the Smtp server username for authentication 
        /// </summary>
        public string SmtpUsername { get; set; }
        /// <summary>
        /// Stores the Smtp server password for authentication  
        /// </summary>
        public string SmtpPassword { get; set; }
        /// <summary>
        /// Stores the value for bypassing the server authentication 
        /// </summary>
        public bool ByPassCertificate { get; set; } = false;
        /// <summary>
        /// Stores the sender address for sending message 
        /// </summary>
        public string Sender { get; set; }
    }

    public class SMTPResponse
    {
        /// <summary>
        /// Stores the object for sending message 
        /// </summary>
        public object Result { get; set; }
        /// <summary>
        /// Stores the status after sending message 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Stores the error list after sending message 
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = new List<string>();
        /// <summary>
        /// Stores the detail of the exeception 
        /// </summary>
        public Exception ExceptionDetail { get; set; }

        /// <summary>
        /// Generate the success response
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns></returns>
        public static SMTPResponse GetSuccessResponse(object obj)
        {
            return new SMTPResponse
            {
                Success = true,
                Result = obj
            };
        }

        /// <summary>
        /// Generate the error response
        /// </summary>
        /// <param name="messages">The errors</param>
        /// <param name="obj">The object</param>
        /// <returns></returns>
        public static SMTPResponse GetErrorResponse(string message, object obj, Exception ex)
        {
            return new SMTPResponse
            {
                Success = false,
                Result = obj,
                Errors = new List<string>() { message },
                ExceptionDetail = ex
            };
        }

        /// <summary>
        /// Generate the error response
        /// </summary>
        /// <param name="messages">The errors</param>
        /// <param name="obj">The object</param>
        /// <returns></returns>
        public static SMTPResponse GetErrorResponse(IEnumerable<string> messages, object obj, Exception ex)
        {
            return new SMTPResponse
            {
                Success = false,
                Result = obj,
                Errors = messages,
                ExceptionDetail = ex
            };
        }
    }
}
