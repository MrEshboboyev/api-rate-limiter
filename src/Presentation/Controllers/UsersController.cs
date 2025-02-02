using Application.Users.Commands.Login;
using Application.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Presentation.Abstractions;
using Presentation.Contracts.Users;

namespace Presentation.Controllers;

[Route("api/users")]
[EnableRateLimiting("fixed")]
public sealed class UsersController(ISender sender) : ApiController(sender)
{
    #region Login and Registration

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var tokenResult = await Sender.Send(command, cancellationToken);

        return tokenResult.IsFailure ? HandleFailure(tokenResult) : Ok(tokenResult.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FullName);

        var result = await Sender.Send(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : Ok();
    }

    #endregion
    
    #region Example endpoints

    // [EnableRateLimiting("fixed")] // 3 requests are allowed in 10 seconds, then a 429 status code is returned.
    [HttpGet("random-number")]
    public async Task<IActionResult> GetRandomNumber(CancellationToken cancellationToken)
    {
        var rnd = new Random();
        
        return Ok(rnd.Next());
    }
    
    [DisableRateLimiting] // this rate limiter disable all rate limiter policies and this endpoint
    [HttpGet("random-number-two")]
    public async Task<IActionResult> GetRandomNumberTwo(CancellationToken cancellationToken)
    {
        var rnd = new Random();
        
        return Ok(rnd.Next());
    }
    
    #endregion
}