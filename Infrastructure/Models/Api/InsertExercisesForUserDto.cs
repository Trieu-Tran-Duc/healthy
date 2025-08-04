using Infrastructure.Validators;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class InsertExercisesForUserDto
{
    [JsonPropertyName("mail-address")]
    [ApiRequiredAttribute(FieldName = "Mail Address")]
    [MailAddressAttribute]
    public string MailAddress { get; set; } = string.Empty;

    [JsonPropertyName("total-exercises")]
    public int TotalExercises { get; set; }
}
