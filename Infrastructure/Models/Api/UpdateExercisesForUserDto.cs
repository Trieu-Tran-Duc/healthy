using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class UpdateExercisesForUserDto
{
    [JsonPropertyName("user-id")]
    public int UserId { get; set; }

    [JsonPropertyName("exercise-id")]
    public int ExerciseId { get; set; }

    [JsonPropertyName("status")]
    public bool IsComplete { get; set; }
}
