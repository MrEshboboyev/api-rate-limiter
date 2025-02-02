namespace Domain.Shared;

public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    #region Constructors
    
    // Constructor to initialize ValidationResult with errors
    private ValidationResult(Error[] errors)
        : base(default!, false, IValidationResult.ValidationError)
        => Errors = errors;
    
    #endregion
    
    #region Properties

    // Array of errors that occurred during validation
    public Error[] Errors { get; }
    
    #endregion

    #region Methods
    
    // Factory method to create a ValidationResult with errors for a specific type
    public static ValidationResult<TValue> WithErrors(Error[] errors)
        => new(errors);
    
    #endregion
}