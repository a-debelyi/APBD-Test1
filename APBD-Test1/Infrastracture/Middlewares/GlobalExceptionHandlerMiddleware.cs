using APBD_8.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace APBD_8.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            await HandleApiExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private Task HandleApiExceptionAsync(HttpContext context, ApiException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception.StatusCode;
        var problemDetail = new { status = exception.StatusCode, title = exception.Message };
        return context.Response.WriteAsJsonAsync(problemDetail);
    }

    private Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var problemDetail = new
            { status = StatusCodes.Status500InternalServerError, title = "Unexpected error happened" };
        return context.Response.WriteAsJsonAsync(problemDetail);
    }
}