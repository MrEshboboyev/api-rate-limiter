using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Abstractions;

[ApiController]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected readonly ISender Sender = sender;

    /// <summary> 
    /// Handles failure scenarios and returns appropriate HTTP responses. 
    /// </summary> 
    /// <param name="result">The result to handle.</param> 
    /// <returns>An IActionResult representing the HTTP response.</returns>
    protected IActionResult HandleFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            IValidationResult validationResult =>
                BadRequest(
                    CreateProblemDetails(
                        "Validation Error",
                        StatusCodes.Status400BadRequest,
                        result.Error, validationResult.Errors)),
            _ => BadRequest(
                CreateProblemDetails(
                    "Bad Request",
                    StatusCodes.Status400BadRequest,
                    result.Error))
        };
    
    #region Helpers

    /// <summary> 
    /// Creates a ProblemDetails object for detailed error responses. 
    /// </summary> 
    /// <param name="title">The title of the problem.</param> 
    /// <param name="status">The HTTP status code.</param> 
    /// <param name="error">The main error.</param> 
    /// <param name="errors">Optional additional errors.</param> 
    /// <returns>A ProblemDetails object with the specified details.</returns>
    private static ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null) =>
        new()
        {
            Title = title,
            Type = error.Code,
            Detail = error.Message,
            Status = status,
            Extensions = 
            { 
                {
                    nameof(errors), 
                    errors 
                } 
            }
        };
    
    #endregion
}