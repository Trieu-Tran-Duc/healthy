using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Validators;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ApiStringLengthAttribute : StringLengthAttribute
{
    public string FieldName { get; set; } = "This field";

    public ApiStringLengthAttribute(int maximumLength) : base(maximumLength) { }

    /// <summary>
    /// checks if the given value is a string and its length is within the specified maximum length.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString())) return ValidationResult.Success;

        var isValid = base.IsValid(value);

        if (isValid) return ValidationResult.Success;

        string message = $"{FieldName} must be at most {MaximumLength} characters.";
        return new ValidationResult(message);
    }
}