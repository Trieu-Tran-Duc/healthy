using Infrastructure.Models.Web;

namespace Web.Models;

public class HomePageViewModel
{
    public double ExerciseRate { get; set; }
    public string? ExerciseDate { get; set; }
    public List<FoodHistoryModel> HistoryMeals { get; set; } = new List<FoodHistoryModel>();
}
