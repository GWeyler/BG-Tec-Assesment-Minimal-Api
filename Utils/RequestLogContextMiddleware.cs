using Microsoft.IdentityModel.Tokens;
using Serilog.Context;
using Serilog.Data;
using System.Net.NetworkInformation;

namespace BG_Tec_Assesment_Minimal_Api.Utils
{
    public class RequestLogContextMiddleware
    {

        private readonly RequestDelegate _next;

        private readonly ILogger<RequestLogContextMiddleware> _logger;

        public RequestLogContextMiddleware(RequestDelegate next, ILogger<RequestLogContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async  Task InvokeAsync(HttpContext context)
        {
            
            using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
            {
                await _next(context);
            }
        }
    }
}
