using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    public RequestDelegate _next { get; }
    public ILogger<ExceptionMiddleware> _logger { get; }
    public IHostEnvironment _env { get; }
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
                                IHostEnvironment env)
    {
            _logger = logger;
            _next = next;
            _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
            ? new ApiExceptions(context.Response.StatusCode, ex.Message , ex.StackTrace?.ToString())
            : new ApiExceptions(context.Response.StatusCode, "Internet Server");

            var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
  }
}