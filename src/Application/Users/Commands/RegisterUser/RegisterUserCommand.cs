using Application.Abstractions.Messaging;

namespace Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FullName) : ICommand;