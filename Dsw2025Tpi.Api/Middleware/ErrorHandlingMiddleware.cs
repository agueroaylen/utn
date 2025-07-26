using System.Net;
using System.Text.Json;

namespace Dsw2025Tpi.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Sigue con el resto del pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado"); // Log interno
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "Ocurrió un error inesperado en el servidor."
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
