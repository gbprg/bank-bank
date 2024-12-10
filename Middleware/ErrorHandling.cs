using System.Text.Json;
using transfer_bank.Helpers;

namespace transfer_bank.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private const string GenericErrorMessage = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (UnauthorizedException ex)
        {
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (ArgumentException ex)
        {
            await HandleArgumentExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = new
        {
            Message = ex.Message,
            ErrorCode = "ValidationError"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }

    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        var response = new
        {
            Message = ex.Message,
            ErrorCode = "NotFoundError"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }

    private static Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var response = new
        {
            Message = ex.Message,
            ErrorCode = "UnauthorizedError"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }

    private static Task HandleArgumentExceptionAsync(HttpContext context, ArgumentException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = new
        {
            Message = ex.Message,
            ErrorCode = "ArgumentError"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }

    private static Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            Message = GenericErrorMessage,
            ErrorCode = "InternalServerError"
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
