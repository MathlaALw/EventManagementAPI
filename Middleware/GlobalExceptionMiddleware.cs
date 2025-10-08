using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementAPI.Middleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {

        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext ctx, Exception ex)
        {
            // Map exception -> status code + title
            var (status, title) = ex switch
            {
                ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid argument"),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Not found"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Concurrency conflict"),
                DbUpdateException => (HttpStatusCode.BadRequest, "Database update error"),
                BadHttpRequestException => (HttpStatusCode.BadRequest, "Bad request"),
                _ => (HttpStatusCode.InternalServerError, "Server error")
            };

            var traceId = ctx.TraceIdentifier;

            // Log with structured properties
            _logger.LogError(ex,
                "Unhandled exception: {Title} | Status: {Status} | Path: {Path} | TraceId: {TraceId}",
                title, (int)status, ctx.Request.Path, traceId);

            var problem = new ProblemDetails
            {
                Title = title,
                Detail = ex.Message,                    // keep short; avoid leaking secrets
                Status = (int)status,
                Instance = ctx.Request.Path,
                Type = "about:blank"                    // You can put a doc URL here
            };

            // Include a trace id to correlate logs <-> response
            problem.Extensions["traceId"] = traceId;

            // Optional: include inner exception message in Development only
            if (ctx.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true && ex.InnerException is not null)
                problem.Extensions["inner"] = ex.InnerException.Message;

            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await ctx.Response.WriteAsync(json);
        }

    }
}
