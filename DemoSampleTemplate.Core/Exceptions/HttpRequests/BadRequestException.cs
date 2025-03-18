using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.HttpRequests
{
    [Serializable]
    public class BadRequestException : BaseRequestException
    {
        public BadRequestException(string message = "BadRequest", string errorCode = "ERR400", Exception? exception = null)
            : base(message, errorCode, StatusCodes.Status400BadRequest, exception) { }
    }
}
