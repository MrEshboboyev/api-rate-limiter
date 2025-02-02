using Application.Users.Commands.Login;
using Application.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;
using Presentation.Contracts.Users;

namespace Presentation.Controllers;

[Route("api/users")]
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

    [HttpGet("random-number")]
    public async Task<IActionResult> GetRandomNumber(CancellationToken cancellationToken)
    {
        var rnd = new Random();
        
        return Ok(rnd.Next());
    }
    
    #endregion
}