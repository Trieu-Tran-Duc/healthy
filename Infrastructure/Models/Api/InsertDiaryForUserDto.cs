using Infrastructure.Validators;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class InsertDiaryForUserDto
{
    [JsonPropertyName("mail-address")]
    [ApiRequiredAttribute(FieldName = "Mail Address")]
    [MailAddressAttribute]
    public string MailAddress { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    [ApiStringLengthAttribute(255, FieldName ="Title")]
    public string? Title { get; set; }

    [JsonPropertyName("content")]
    [ApiStringLengthAttribute(500, FieldName = "Content")]
    public string? Content { get; set; }
}
