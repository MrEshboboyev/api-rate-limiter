namespace Presentation.Contracts.Users;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FullName);