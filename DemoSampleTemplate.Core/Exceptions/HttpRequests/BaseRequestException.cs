using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.HttpRequests
{
    public class BaseRequestException : Exception
    {
        public BaseRequestException() : base() { }

        public BaseRequestException(string message) : base(message) { }

        public BaseRequestException(string message, string errorCode = "", int statusCode = StatusCodes.Status500InternalServerError) : base(message)
        {
            _httpStatusCode = statusCode;
            _errorCode = errorCode;
        }


        public int StatusCode { get => _httpStatusCode; }
        public string ErrorCode { get => _errorCode; }

        private int _httpStatusCode;
        private string _errorCode = string.Empty;
    }
}
