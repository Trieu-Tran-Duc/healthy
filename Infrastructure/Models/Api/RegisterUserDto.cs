using Infrastructure.Validators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Api;

public class RegisterUserDto
{
    [JsonPropertyName("first-name")]
    [Required]
    [StringLength(64)]
    public string? FirstName { get; set; }

    [JsonPropertyName("last-name")]
    [Required]
    [StringLength(64)]
    public string? LastName { get; set; }

    [JsonPropertyName("mail-address")]
    [Required]
    [StringLength(256)]
    [MailAddress]
    public string? MailAddress { get; set; }

    [JsonPropertyName("password")]
    [Required]
    [StringLength(128)]
    public string? LoginPassword { get; set; }
}
