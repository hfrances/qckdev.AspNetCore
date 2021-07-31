using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Middleware
{
    sealed class ExceptionHandlerResponseMiddleware
    {
        RequestDelegate Next { get; }
        ILogger<ExceptionHandlerResponseMiddleware> Logger { get; }

        public ExceptionHandlerResponseMiddleware(RequestDelegate next, ILogger<ExceptionHandlerResponseMiddleware> logger)
        {
            this.Next = next;
            this.Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.Next(context);
            }
            catch (Exception ex)
            {
                await HandlerExceptionAsync(context, ex, this.Logger);
            }
        }

        private async Task HandlerExceptionAsync(HttpContext context, Exception ex, ILogger<ExceptionHandlerResponseMiddleware> logger)
        {
            SerializedError error;
            int errorCode;

            switch (ex)
            {
                case HttpHandledException httpe:
                    errorCode = (int)httpe.ErrorCode;
                    logger.LogError(ex, $"Handled error from server ({errorCode})");
                    error = SerializeErrors(httpe);
                    break;
                //case Exception e:
                default:
                    errorCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(ex, "Error from server");
                    error = SerializeErrors(ex);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;
            if (error != null)
            {
                var result = JsonConvert.SerializeObject(new { error },
                    new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                await context.Response.WriteAsync(result);
            }
        }

        private static SerializedError SerializeErrors(Exception ex)
        {
            SerializedError error;

            if (ex is AggregateException aggregateException)
            {
                error = new SerializedAggregateError();

                ((SerializedAggregateError)error).InnerErrors =
                    aggregateException.InnerExceptions
                        .Select(SerializeErrors);
            }
            else
            {
                error = new SerializedError();
            }

            error.Message = ex.Message;
            if (ex is HttpHandledException httpe)
            {
                error.Content = httpe.Content;
            }
            if (ex.InnerException != null)
            {
                error.InnerError = SerializeErrors(ex.InnerException);
            }
            return error;
        }

        private class SerializedError
        {
            public string Message { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public dynamic Content { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public SerializedError InnerError { get; set; }

        }

        private class SerializedAggregateError : SerializedError
        {
            public IEnumerable<SerializedError> InnerErrors { get; set; }
        }

    }
}
