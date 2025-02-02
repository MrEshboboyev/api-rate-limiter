using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace App.Middlewares;

public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception.
            logger.LogError(ex, message: ex.Message);

            // Set the response status code to 500 Internal Server Error.
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a ProblemDetails object to represent the error.
            ProblemDetails problem = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server error",
                Detail = "An internal server error has occured"
            };

            // Serialize the ProblemDetails object to JSON.
            var json = JsonSerializer.Serialize(problem);

            // Set the response content type to application/json.
            context.Response.ContentType = "application/json";

            // Write the JSON error response to the HTTP response.
            await context.Response.WriteAsync(json);
        }
    }
}