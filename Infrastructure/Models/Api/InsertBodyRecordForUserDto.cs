using Infrastructure.Validators;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class InsertBodyRecordForUserDto
{
    [JsonPropertyName("mail-address")]
    [ApiRequiredAttribute(FieldName = "Mail Address")]
    [MailAddressAttribute]
    public string MailAddress { get; set; } = string.Empty;

    [JsonPropertyName("record-date")]
    [ApiDateFormatAttribute]
    public DateTime RecordDate { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }
}
