using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class GetExercisesByUserModel
{
    [JsonPropertyName("exercise-id")]
    public int Id { get; set; }

    [JsonPropertyName("exercise-date")]
    public DateTime ExerciseDate { get; set; }

    [JsonPropertyName("exercise-name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("duration-minutes")]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("calories")]
    public int? Calories { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
