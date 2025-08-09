using Infrastructure.Validators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class InsertExercisesForUserDto
{
    [JsonPropertyName("mail-address")]
    [ApiRequiredAttribute(FieldName = "Mail Address")]
    [MailAddressAttribute]
    public string MailAddress { get; set; } = string.Empty;

    [JsonPropertyName("exercise-date")]
    [ApiDateFormatAttribute]
    public string ExerciseDate { get; set; }

    [JsonPropertyName("exercise-name")]
    [ApiRequired]
    [ApiStringLength(255)]
    public string Name { get; set; }

    [JsonPropertyName("duration-minutes")]
    [Required]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("calories")]
    public int? Calories { get; set; }

    [JsonPropertyName("description")]
    [StringLength(1000)]
    public string? Description { get; set; }
}
