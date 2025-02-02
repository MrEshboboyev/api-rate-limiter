using Domain.Shared;

namespace Domain.Errors;

public static class DomainErrors
{
    #region User
    
    #region Entities
    
    public static class User
    {
        public static readonly Error EmailAlreadyInUse = new(
            "User.EmailAlreadyInUse",
            "The specified email is already in use");

        public static readonly Func<Guid, Error> NotFound = id => new Error(
            "User.NotFound",
            $"The user with the identifier {id} was not found.");

        public static readonly Error NotExist = new(
            "Users.NotExist",
            $"There is no users");

        public static readonly Error InvalidCredentials = new(
            "User.InvalidCredentials",
            "The provided credentials are invalid");
    }

    #endregion
    
    #region Value Objects
    
    public static class Email
    {
        public static readonly Error Empty = new(
            "Email.Empty",
            "Email is empty");
        public static readonly Error InvalidFormat = new(
            "Email.InvalidFormat",
            "Email format is invalid");
    }

    public static class FullName
    {
        public static readonly Error Empty = new(
            "FirstName.Empty",
            "First name is empty");
        public static readonly Error TooLong = new(
            "LastName.TooLong",
            "FirstName name is too long");
    }

    public static class RateLimit
    {
        public static readonly Func<int, Error> InvalidMaxRequests = maxRequests => new Error(
            "RateLimit.InvalidMaxRequests",
            $"MaxRequests {maxRequests} must be greater than zero.");
        
        public static readonly Func<TimeSpan, Error> InvalidTimeWindow = timeWindow => new Error(
            "RateLimit.InvalidTimeWindow",
            $"TimeWindow {timeWindow} must be greater than zero.");
        
        public static readonly Error Exceeded = new Error(
            "RateLimit.Exceeded",
            "Too many requests. Please try again later.");
    }
    
    #endregion
    
    #endregion
}