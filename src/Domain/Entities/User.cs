using Domain.Errors;
using Domain.Events;
using Domain.Primitives;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class User : AggregateRoot, IAuditableEntity
{
    #region Constructors

    private User(
        Guid id,
        Email email,
        string passwordHash,
        FullName fullName) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    #endregion

    #region Properties

    public string PasswordHash { get; set; }
    public FullName FullName { get; set; }
    public Email Email { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    #endregion

    #region Factory Methods

    public static User Create(
        Guid id,
        Email email,
        string passwordHash,
        FullName fullName
    )
    {
        #region Create new User

        var user = new User(
            id,
            email,
            passwordHash,
            fullName);

        #endregion

        #region Domain Events

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            user.Id));

        #endregion

        return user;
    }

    #endregion

    #region Own Methods

    public void ChangeName(FullName fullName)
    {
        #region Checking new values are equals old valus

        if (!FullName.Equals(fullName))
        {
            RaiseDomainEvent(new UserNameChangedDomainEvent(
                Guid.NewGuid(),
                Id));
        }

        #endregion

        #region Update fields

        FullName = fullName;

        #endregion
    }

    #endregion
}