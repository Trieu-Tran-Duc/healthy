using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Utilities;

namespace ApplicationCore.Service;

public interface IFoodService
{
    Task GenerateFoodList();
}

/// <summary>
/// food service for food operations
/// </summary>
public class FoodService : IFoodService
{
    private readonly HealtyDbContext _healtyDbContext;

    public FoodService(HealtyDbContext healtyDbContext)
    {
        _healtyDbContext = healtyDbContext;
    }

    /// <summary>
    ///  automatically generate food list from food data source.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task GenerateFoodList()
    {
        await GenerateFoods(GetMorningFoods(), MealType.Morning);
        await GenerateFoods(GetLunchFoods(), MealType.Lunch);
        await GenerateFoods(GetDinnerFoods(), MealType.Dinner);
        await GenerateFoods(GetSnackFoods(), MealType.Snack);
    }

    /// <summary>
    /// common method save foods to database.
    /// </summary>
    /// <param name="foods"></param>
    /// <param name="mealType"></param>
    /// <returns></returns>
    private async Task GenerateFoods(Dictionary<string, string> foods, MealType mealType)
    {
        var foodEntities = foods.Select(food => new Food
        {
            Name = food.Key,
            PhotoUrl = food.Value,
            MealType = mealType
        }).ToList();

        await _healtyDbContext.Food.AddRangeAsync(foodEntities);
        await _healtyDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// dummy data for morning foods.
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetMorningFoods() => new()
    {
        { "焼き魚とご飯", "8a5b5b54-280d-43b4-a914-bd7f97447fc9.png" },
        { "納豆ご飯", "a46160a2-7c90-4dc7-bb6f-a69ae0ba5a2d.jpg" },
        { "味噌汁", "acb4a1a5-4240-42f0-b84f-d5c0b845b98f.jpg" },
        { "卵焼き", "d1f889da-f883-450e-9a29-5fbe00be21b3.jpg" },
        { "おにぎり", "df3abdc1-0ed8-4e84-85c4-17b987d63e94.jpg" },
        { "サンドイッチ", "e0cba44a-dec3-4167-b2c1-56b167b1e218.jpg" },
        { "おかゆ", "e863cf4e-ede9-4eb7-b30b-ff7a0a45d8d2.jpg" },
        { "お好み焼き", "f2ccdf56-da7d-4a3a-8ae5-9df043092a27.png" },
    };

    /// <summary>
    /// dummy data for lunch foods.
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetLunchFoods() => new()
    {
        { "カレーライス", "0f9b6253-ba50-45c5-a7c0-bc8446dca987.jpg" },
        { "豚カツ", "2ed0040f-422a-4e5a-90be-a5540c752d6e.jpg" },
        { "ラーメン", "9e048ab7-3c32-4ab7-b34a-b9d39203fc6b.jpg" },
        { "鮭いくら丼", "57a19321-8727-4a14-bec1-d7a7069b349d.jpg" },
        { "オムライス", "94f68f11-9ba0-4dc8-b73f-551e89279cc6.jpg" },
        { "冷たいそば", "c17a0b84-8092-4d90-9856-6446a25f0ea5.jpg" },
        { "焼き魚定食", "df2fd1e3-4a90-4be0-bc00-1b6d16c1658e.jpg" },
        { "天ぷらうどん", "ef56e790-23a3-44db-9403-86426d47e714.jpg" },
    };

    /// <summary>
    /// dummy data for dinner foods.
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetDinnerFoods() => new()
    {
        { "寿司", "1f2036d8-c45d-419a-87ff-e4780e8f4b6e.jpg" },
        { "天ぷら", "55ff30c9-705d-4adb-8d75-a4d75830ac0b.jpg" },
        { "焼き魚", "93e77709-a8e9-42c7-83fc-1b5d48675335.jpg" },
        { "味噌汁", "168df70b-b7a4-4446-a35c-39d045a89a56.jpg" },
        { "うどん", "4113f6f7-bca4-4c7e-ac64-f03e10b93af7.jpg" },
        { "親子丼", "568062e7-5d92-4aaa-8593-795a263ae6ea.jpg" },
        { "焼き鳥", "afa0c9c0-0239-4adb-9dd8-c43a7356609f.jpg" },
        { "鯖の味噌煮", "f143931e-3c03-4b0f-976a-7d0d24476e1b.jpg" },
    };

    /// <summary>
    /// dummy data for snack foods.
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetSnackFoods() => new()
    {
        { "煎餅", "1aa1ed72-eb58-49a4-91e8-25a52193e237.jpg" },
        { "大福餅", "2b302331-6a60-45d7-af20-c5b1fe3e3c23.jpg" },
        { "ポテトチップス", "6fdfcda2-7b49-4abf-b975-1ed9ff270573.jpg" },
        { "ポッキー", "7a947da8-513d-437f-9d6c-e90c8921fbcb.jpg" },
        { "たこ焼き", "661ba422-a8ea-4e15-af79-311f2b00606e.jpg" },
        { "たい焼き", "767c7485-3138-41eb-9e2a-b3d8fad3de58.png" },
        { "のり塩スティック", "ba64cc22-3a15-45a9-b4a1-e9740e9d08f3.png" },
        { "枝豆スナック", "cf36654c-ace6-493a-a7c7-53a0bc898cf1.jpg" },
    };
}
