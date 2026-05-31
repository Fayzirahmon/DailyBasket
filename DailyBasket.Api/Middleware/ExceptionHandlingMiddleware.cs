using System.Text.Json;
using DailyBasket.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            await WriteErrorAsync(context, exception.StatusCode, exception.Message);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Database update failed.");
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "A database error occurred while processing the request.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API exception.");
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred while processing the request.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = new
        {
            statusCode,
            message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}
