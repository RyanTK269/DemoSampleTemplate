using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.HttpRequests
{
    [Serializable]
    public abstract class BaseRequestException : Exception
    {
        private int _httpStatusCode;
        private string _errorCode;

        protected BaseRequestException(string message = "", string errorCode = "ERR", int httpStatusCode = StatusCodes.Status500InternalServerError, Exception? exception = null)
            : base(message, exception)
        {
            this._httpStatusCode = httpStatusCode;
            this._errorCode = errorCode;
        }

        public virtual int HttpStatusCode { get => this._httpStatusCode; }
        public virtual string ErrorCode { get => this._errorCode; set => this._errorCode = value; }
        public virtual string ErrorMessage { get => string.Format("{0} - {1}", this.ErrorCode, this.Message); }
    }
}
