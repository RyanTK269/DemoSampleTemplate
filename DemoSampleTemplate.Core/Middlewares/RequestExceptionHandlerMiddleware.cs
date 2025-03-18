using DemoSampleTemplate.Core.DataObjects.Http;
using DemoSampleTemplate.Core.Exceptions.HttpRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DemoSampleTemplate.Core.Middlewares
{
    public class RequestExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestExceptionHandlerMiddleware> _logger;

        public RequestExceptionHandlerMiddleware(RequestDelegate next, ILogger<RequestExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation(string.Format("Path: {0} - Method: {1} - Started", context.Request.Path, context.Request.Method));
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("Path: {0} - Method: {1} - Error. {2}", context.Request.Path, context.Request.Method, ex.Message));
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new ResponseModel
                {
                    Message = ex.Message,
                    Details = ex.InnerException.Message
                };

                switch (ex)
                {
                    case BaseRequestException baseEx:
                        responseModel = new ResponseModel
                        {
                            ErrorCode = baseEx.ErrorCode,
                            Message = baseEx.ErrorMessage,
                            Details = ex?.InnerException?.Message
                        };
                        response.StatusCode = baseEx.HttpStatusCode;
                        break;
                    default:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }
                var result = JsonConvert.SerializeObject(responseModel);
                await response.WriteAsync(result);
            }
            finally
            {
                _logger.LogInformation(string.Format("Path: {0} - Method: {1} - Finished", context.Request.Path, context.Request.Method));
            }
        }
    }
}
