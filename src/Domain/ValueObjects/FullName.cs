using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    #region Constants
    
    public const int MaxLength = 50; // Maximum length for an FullName
    
    #endregion
    
    #region Constructors
    
    private FullName(string value)
    {
        Value = value;
    }
    
    #endregion
    
    #region Properties
    
    public string Value { get; }
    
    #endregion

    #region Factory Methods

    /// <summary> 
    /// Creates a FirstName instance after validating the input. 
    /// </summary> 
    /// <param name="fullName">The first name string to create the FirstName value object from.</param> 
    /// <returns>A Result object containing the FirstName value object or an error.</returns>
    public static Result<FullName> Create(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure<FullName>(
                DomainErrors.FullName.Empty);
        }
        
        if (fullName.Length > MaxLength)
        {
            return Result.Failure<FullName>(
                DomainErrors.FullName.TooLong);
        }
        
        return Result.Success(new FullName(fullName));
    }
    
    #endregion

    #region Overrides
    
    /// <summary> 
    /// Returns the atomic values of the FirstName object for equality checks. 
    /// </summary>
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
    
    #endregion
}