using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PhotoShowdownBackend.Utils;

namespace PhotoShowdownBackend.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HandleExceptionAttribute : TypeFilterAttribute
{
    public HandleExceptionAttribute() : base(typeof(ExceptionFilter))
    {
    }

    private class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionFilter(ILogger<ExceptionFilter> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception with class and method name
            _logger.LogError(context.Exception, "Error at: {errorLocation}", context.ActionDescriptor.DisplayName);

            // Set the response
            APIResponse response = _environment.IsDevelopment()
                ? new APIResponse<string>()
                {
                    Data = context.Exception.StackTrace,
                    IsSuccess = false,
                    Message = context.Exception.Message,
                }
                : APIResponse.ServerError;

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
            };

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }
    }
}
