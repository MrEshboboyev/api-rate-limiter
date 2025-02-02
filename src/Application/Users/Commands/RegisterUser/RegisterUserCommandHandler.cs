using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterUserCommand>
{
    public async Task<Result> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var (email, password, fullName) = request;
        
        #region Checking Email is Unique

        // Validate and create the Email value object
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(
                emailResult.Error);
        }

        // Check if the email is already in use
        if (!await userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
        {
            return Result.Failure(
                DomainErrors.User.EmailAlreadyInUse);
        }

        #endregion
        
        #region Prepare value objects (fullName)
        
        var fullNameResult = FullName.Create(fullName);
        if (fullNameResult.IsFailure)
        {
            return Result.Failure(
                fullNameResult.Error);
        }
        
        #endregion
        
        #region Password hashing

        // Hash the user's password
        var passwordHash = passwordHasher.Hash(password);

        #endregion
        
        #region Create new user

        // Create a new User entity with the provided details
        var user = User.Create(
            Guid.NewGuid(),
            emailResult.Value,
            passwordHash,
            fullNameResult.Value);

        #endregion

        #region Add and Update database

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success();
    }
}