using Infrastructure.Models.Api;
using Infrastructure.Models.Web;

namespace Web.Models;

public class ChallengeViewModel
{
    public string ExerciseDate { get; set; } = string.Empty;
    public List<GetExercisesByUserModel> Exercises { get; set; } = new List<GetExercisesByUserModel>();
    public List<DiariesModel> Diaries { get; set; } = new List<DiariesModel>();
}
