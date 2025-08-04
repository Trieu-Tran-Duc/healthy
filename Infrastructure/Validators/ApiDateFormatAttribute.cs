using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace Infrastructure.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ApiDateFormatAttribute : ValidationAttribute
{
    private const string CONST_OUTPUT_DATE_FORMAT_YYYYMMDD = "yyyy-MM-dd";
    public ApiDateFormatAttribute() { }

    /// <summary>
    /// validataion attribute for date format.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString())) return ValidationResult.Success;

        if (
            value?.ToString()?.Length == CONST_OUTPUT_DATE_FORMAT_YYYYMMDD.Length
            && DateTime.TryParseExact(value.ToString(), 
                CONST_OUTPUT_DATE_FORMAT_YYYYMMDD, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None,
                out var parsedDate))
        {
            var today = DateTime.Today;
            var minDate = today.AddDays(-7);

            if (parsedDate <= today && parsedDate >= minDate)
                return ValidationResult.Success;

            return new ValidationResult(
                $"Date must be between {minDate:yyyy-MM-dd} and {today:yyyy-MM-dd}.");
        }

        return new ValidationResult($"Date must be in format {CONST_OUTPUT_DATE_FORMAT_YYYYMMDD}");
    }
}
