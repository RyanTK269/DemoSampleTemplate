using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.Http
{
    public class ResponseModel
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string? Details { get; set; } = string.Empty;
    }
}
