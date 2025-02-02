using FluentValidation;

namespace Application.Users.Commands.RegisterUser;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .NotNull()
            .WithMessage("Password cannot be null.");
        // could be other validations for password in this case
        // Example : Least 8 characters and more

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("FullName is required.");
    }
}