using System.Net;
using System.Text.Json;

namespace ProductManagementApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException uae)
            {
                _logger.LogWarning(uae, "Unauthorized access attempt.");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new { message = "You are not authorized to access this resource." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (ArgumentNullException ane)
            {
                _logger.LogWarning(ane, "Bad request due to missing argument.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var response = new { message = "One or more required arguments were null." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                    ? new { message = ex.Message, stackTrace = ex.StackTrace }
                    : new { message = "Internal Server Error", stackTrace = (string)null };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
            }
        }

    }
}
