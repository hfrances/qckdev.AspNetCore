using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using qckdev.AspNetCore.Exceptions;
using qckdev.Net;
using qckdev.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Middlewares
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
            int? errorCode = null;

            switch (ex)
            {
                case ApiException apiex:
                    errorCode = (int)apiex.ErrorCode;
                    logger.LogError(ex, $"Handled API error ({errorCode})");
                    logger.LogTrace((string)JsonConvert.SerializeObject(SerializeTrace(apiex)));
                    error = SerializeErrors(apiex);
                    break;
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
            context.Response.StatusCode = errorCode ?? (int)HttpStatusCode.InternalServerError;
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
            if (ex is ApiException apiex)
            {
                error.Content = apiex.Content;
            }
            else if (ex is FetchFailedException httpf)
            {
                error.Content = httpf.Content;
            }
            else if (ex is HttpHandledException httpe)
            {
                error.Content = httpe.Content;
            }
            if (ex.InnerException != null)
            {
                error.InnerError = SerializeErrors(ex.InnerException);
            }
            return error;
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
            else if (ex is ApiException apiex)
            {
                error = new
                {
                    message = apiex.Message,
                    resourceId = apiex.ResourceId,
                    parameters = apiex.Parameters,
                    errorCode = apiex.ErrorCode,
                    content = apiex.Content,
                    innerError = (ex.InnerException == null ? null : SerializeTrace(ex.InnerException))
                };
            }
            else if (ex is FetchFailedException httpf)
            {
                error = new
                {
                    requestUri = httpf.RequestUri,
                    message = httpf.Message,
                    errorCode = httpf.StatusCode,
                    error = httpf.Content,
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
            public string Message { get; set; } = string.Empty;

#if NETCOREAPP3_1
            [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
#else
            [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
#endif
            public dynamic? Content { get; set; }

#if NETCOREAPP3_1
            [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
#else
            [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
#endif
            public SerializedError? InnerError { get; set; }

        }

        private class SerializedAggregateError : SerializedError
        {
            public IEnumerable<SerializedError>? InnerErrors { get; set; }
        }

    }
}
