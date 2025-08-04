using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ApiRequiredAttribute : RequiredAttribute
{
    public string FieldName { get; set; } = "This field";

    /// <summary>
    /// checks if the given value is not null or empty.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var isValid = base.IsValid(value);

        if (isValid) return ValidationResult.Success;

        return new ValidationResult($"{FieldName} is required.");
    }
}
