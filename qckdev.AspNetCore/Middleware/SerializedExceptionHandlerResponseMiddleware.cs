using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using qckdev.Net.Http;
using qckdev.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace qckdev.AspNetCore.Middleware
{
    sealed class SerializedExceptionHandlerResponseMiddleware
    {
        RequestDelegate Next { get; }
        ILogger<SerializedExceptionHandlerResponseMiddleware> Logger { get; }

        public SerializedExceptionHandlerResponseMiddleware(RequestDelegate next, ILogger<SerializedExceptionHandlerResponseMiddleware> logger)
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

        private async Task HandlerExceptionAsync(HttpContext context, Exception ex, ILogger<SerializedExceptionHandlerResponseMiddleware> logger)
        {
            SerializedError error;
            int errorCode;

            switch (ex)
            {
                case FetchFailedException httpf:
                    errorCode = (int)httpf.StatusCode;
                    logger.LogError(ex, $"Handled error from server ({errorCode})");
                    logger.LogTrace((string)JsonConvert.SerializeObject(SerializeTrace(httpf)));
                    error = SerializeErrors(httpf);
                    break;
                case HttpHandledException httpe:
                    errorCode = (int)httpe.ErrorCode;
                    logger.LogError(ex, $"Handled error from server ({errorCode})");
                    logger.LogTrace((string)JsonConvert.SerializeObject(SerializeTrace(httpe)));
                    error = SerializeErrors(httpe);
                    break;
                default:
                    errorCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(ex, "Error from server");
                    logger.LogTrace((string)JsonConvert.SerializeObject(SerializeTrace(ex)));
                    error = SerializeErrors(ex);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;
            if (error != null)
            {
                var result = JsonConvert.SerializeObject(new { error });
                await context.Response.WriteAsync(result);
            }
        }

        private static SerializedError SerializeErrors(Exception ex)
        {
            SerializedError error;

            if (ex is AggregateException aggregateException)
            {
                error = new SerializedAggregateError()
                {
                    InnerErrors = aggregateException.InnerExceptions.Select(SerializeErrors)
                };
            }
            else
            {
                error = new SerializedError();
            }

            if (ex is FetchFailedException httpf)
            {
                error.Content = httpf.Error;
            }
            else if (ex is HttpHandledException httpe)
            {
                error.Content = httpe.Content;
            }

            if (ex.InnerException != null)
            {
                error.InnerError = SerializeErrors(ex.InnerException);
            }

            error.Message = ex.Message;
            error.TraceId = GetTraceId(error.Content);
            if (error.TraceId == Guid.Empty)
            {
                error.TraceId = Guid.NewGuid();
            }

            return error;
        }

        private static Guid GetTraceId(dynamic content)
        {
            return Guid.Empty;
        }

        private static dynamic SerializeTrace(Exception ex)
        {
            dynamic error;

            if (ex is AggregateException aggregateException)
            {
                error = new
                {
                    ex.Message,
                    InnerErrors = aggregateException.InnerExceptions.Select(SerializeTrace)
                };
            }
            else if (ex is FetchFailedException httpf)
            {
                error = new
                {
                    requestUri = httpf.RequestUri,
                    message = httpf.Message,
                    errorCode = httpf.StatusCode,
                    error = httpf.Error,
                    innerError = (ex.InnerException == null ? null : SerializeTrace(ex.InnerException))
                };
            }
            else if (ex is HttpHandledException httpe)
            {
                error = new
                {
                    message = httpe.Message,
                    errorCode = httpe.ErrorCode,
                    content = httpe.Content,
                    innerError = (ex.InnerException == null ? null : SerializeTrace(ex.InnerException))
                };
            }
            else
            {
                error = new
                {
                    message = ex.Message,
                    innerError = (ex.InnerException == null ? null : SerializeTrace(ex.InnerException))
                };
            }
            return error;
        }

        private class SerializedError
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public Guid TraceId { get; set; }

            public string Message { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public dynamic Content { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public SerializedError InnerError { get; set; }

        }

        private class SerializedAggregateError : SerializedError
        {
            public IEnumerable<SerializedError> InnerErrors { get; set; }
        }

    }
}
