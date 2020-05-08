using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OrderManager.Console.Application
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        [DebuggerStepThrough]
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) =>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogDebug($"Handling {typeof(TRequest).Name}");
            var response = await next();
            _logger.LogDebug($"Handled {typeof(TRequest).Name}");
            return response;
        }
    }
}
