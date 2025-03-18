using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.HttpRequests
{
    [Serializable]
    public class NotFoundRequestException : BaseRequestException
    {
        public NotFoundRequestException(string message = "Not found", string errorCode = "ERR404") : base(message, errorCode, StatusCodes.Status404NotFound) { }
    }
}
