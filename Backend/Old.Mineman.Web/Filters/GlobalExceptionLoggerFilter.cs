using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Filters
{
    public class GlobalExceptionLoggerFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionLoggerFilter> _logger;

        public GlobalExceptionLoggerFilter(ILogger<GlobalExceptionLoggerFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(), context.Exception, "Uncaught exception intercepted by filter");
        }
    }
}
