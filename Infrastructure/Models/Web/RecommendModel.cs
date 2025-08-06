namespace Infrastructure.Models.Web;

public class RecommendModel
{
    public string ImageUrl { get; set; } = string.Empty; 
    public string RecommendDate { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public List<string> HashTag { get; set; } = new List<string>();
}
