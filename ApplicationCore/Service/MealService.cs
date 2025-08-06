using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Web;
using Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IMealService
{
    Task<List<FoodHistoryModel>> GetMealsByUserId(int userId, int pageSize, int pageIndex, MealType? mealType);
    Task GenerateMeals(string email, DateTime dateTime);
}

/// <summary>
/// meal service for meal operations
/// </summary>
public class MealService : IMealService
{
    private readonly HealtyDbContext _dbContext;

    private readonly Dictionary<string, string> _imageFoodPath = new()
    {
        { MealType.Morning.GetDescription(), "/images/food/morning/" },
        { MealType.Lunch.GetDescription(), "/images/food/lunch/" },
        { MealType.Dinner.GetDescription(), "/images/food/dinner/" },
        { MealType.Snack.GetDescription(), "/images/food/snack/" }
    };

    public MealService(HealtyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// dummy method to generate meals for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task GenerateMeals(string email, DateTime dateTime)
    {
        var checkUser = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.MailAddress == email && u.DeletedAt == null);

        if (checkUser == null)
            throw new ArgumentException($"User with mail address {email} does not exist.");

        var mealTypes = Enum.GetValues(typeof(MealType)).Cast<MealType>().ToList();
        foreach (var mealType in mealTypes)
        {
            var food = await GetRandomFood(mealType);
            if (food == null)
                throw new ArgumentException($"No food found for meal type {mealType}.");
           
            var meal = new Meal
            {
                UserId = checkUser.Id,
                FoodId = food.Id,
                MealDate = dateTime,
                MealType = mealType,
                CreatedUserId = checkUser.Id
            };
            
           await _dbContext.Meal.AddAsync(meal);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// randomly get a food item based on the meal type.
    /// </summary>
    /// <param name="mealType"></param>
    /// <returns></returns>
    private async Task<Food?> GetRandomFood(MealType mealType)
    {
       return await _dbContext.Food
            .Where(f => f.MealType == mealType)
            .OrderBy(f => Guid.NewGuid())
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// get list of meals by user id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<FoodHistoryModel>> GetMealsByUserId(int userId, int pageSize, int pageIndex, MealType? mealType)
    {
        var listMeal = await _dbContext.Meal
            .Join(
                _dbContext.Food, 
                meal => meal.FoodId, 
                food => food.Id,
                (meal, food) => new { meal, food }
            )
            .Where(m => m.meal.UserId == userId 
                     && m.meal.DeletedAt == null
                     && (mealType == null || m.meal.MealType == mealType)
             )
            .AsNoTracking()
            .Select(s => new FoodHistoryModel
            {
                ImageUrl = s.food.PhotoUrl,
                MealDate = s.meal.MealDate.ToString("MM.dd"),
                MealType = s.meal.MealType.GetDescription(),
                FoodName = s.food.Name,
                Position = (int)s.meal.MealType,
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

       
        foreach (var meal in listMeal)
        {
            if (_imageFoodPath.TryGetValue(meal.MealType, out var path))
            {
                meal.ImageUrl = $"{path}{meal.ImageUrl}";
            }
        }

        return listMeal
            .OrderByDescending(m => m.MealDate)
            .ThenBy(m => m.Position).ToList();
    }
}
