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

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception with class and method name
            _logger.LogError(context.Exception, "Error at: {errorLocation}", context.ActionDescriptor.DisplayName);

            // You can customize the response content here if needed
            context.Result = new ObjectResult(APIResponse.ServerError)
            {
                StatusCode = 500,
            };

            // Mark the exception as handled
            context.ExceptionHandled = true;
        }
    }
}
