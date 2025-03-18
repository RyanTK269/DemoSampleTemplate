using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.HttpRequests
{
    [Serializable]
    public class UnauthorizedRequestException : BaseRequestException
    {
        public UnauthorizedRequestException(string message = "Unauthorized", string errorCode = "ERR401") : base(message, errorCode, StatusCodes.Status401Unauthorized) { }
    }
}
