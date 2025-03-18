using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Exceptions.Services
{
    public class BaseServiceException
    {
        public BaseServiceException() { }

        public virtual void ThrowException(Exception ex)
        {
            throw new Exception($"{ex.Message}");
        }

        public virtual void ThrowException(string errorMessage)
        {
            throw new Exception($"{errorMessage}");
        }

        public virtual void ThrowException(string errorMessage, Exception ex)
        {
            throw new Exception($"{errorMessage}");
        }
    }
}
