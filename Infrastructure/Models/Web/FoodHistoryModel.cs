namespace Infrastructure.Models.Web;

public class FoodHistoryModel
{
    public string MealType { get; set; } = string.Empty;
    public string MealDate { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string FoodName { get; set; } = string.Empty;
    public int Position { get; set; }
}
