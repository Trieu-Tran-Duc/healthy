namespace Infrastructure.Models.Api;

public class GetUserModelDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MailAddress { get; set; } = string.Empty;
}
