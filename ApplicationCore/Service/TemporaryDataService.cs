using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Infrastructure.Models.Web;
using Infrastructure.Utilities;

namespace ApplicationCore.Service;

public interface ITemporaryDataService
{
   Task SeedDataAsync();
}

/// <summary>
/// Temporary data service for seeding initial data into the application.
/// </summary>
public class TemporaryDataService : ITemporaryDataService
{
    private readonly HealtyDbContext _healtyDbContext;
    private readonly IUserService _userService;
    private readonly IMealService _mealService;
    private readonly IDiaryService _diaryService;
    private readonly IBodyRecordsService _bodyRecordsService;

    public TemporaryDataService
    (
        HealtyDbContext healtyDbContext,
        IUserService userService,
        IMealService mealService,
        IDiaryService diaryService,
        IBodyRecordsService bodyRecordsService
    )
    {
        _userService = userService;
        _healtyDbContext = healtyDbContext;
        _mealService = mealService;
        _diaryService = diaryService;
        _bodyRecordsService = bodyRecordsService;
    }

    /// <summary>
    /// seed temporary data for the application.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SeedDataAsync()
    {
        // Seed user data
        var newUser = await _userService.RegisterUser(new RegisterUserDto()
        {
            FirstName = "Trieu",
            LastName = "TD",
            MailAddress = "test@gmail.com",
            LoginPassword = "123",
        });

        if (newUser == null)
        {
            throw new ArgumentException($"User with Email {newUser?.MailAddress} already exists.");
        }

        for (DateTime date = DateTime.Now.AddDays(-60); date < DateTime.Now; date = date.AddDays(1))
        {
            //seed exercise data for the user
            await GenerateDefaultExercises(newUser.UserId, 5, date);

            // Seed body records for the user
            await _bodyRecordsService.InsertOrUpdateBodyRecordForUser(new InsertBodyRecordForUserDto()
            {
                MailAddress = newUser.MailAddress,
                Weight = 70 + (date.Day % 5),
                RecordDate = date,
            });
        }

        // seed food data for the user
        await SeedDataFoodList();

        for (DateTime date = DateTime.Now.AddDays(-8); date < DateTime.Now; date = date.AddDays(1))
        {
            // Generate meals for the user
            await _mealService.GenerateMeals(newUser.MailAddress, date);
        }

        // Seed recommend data
        await SeedDataRecommend();

        //seed diary data for the user
        for (int i = 0; i < 12; i++)
        {
            await _diaryService.InsertDiaryForUser(new InsertDiaryForUserDto()
            {
                MailAddress = newUser.MailAddress,
                Title = $"今日のひとこと -- {i}",
                Content = $"{i} -- 今日は少し早起きして、近くの公園を散歩しました。朝の空気がとても気持ちよくて、鳥のさえずりが心を落ち着かせてくれました。最近は仕事が忙しくて、なかなかリラックスする時間がなかったけれど、自然に触れることでリフレッシュできた気がします。これからも毎朝少しだけでも、自分の時間を大切にしたいと思います。",
            });
        }
    }

    #region Dummy data Exercises

    /// <summary>
    /// Automatically generated data exercise of user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task GenerateDefaultExercises(int userId, int totalExercises, DateTime dateTime)
    {
        var random = new Random();
        var exerciseList = new List<Exercise>();

        // Define a list of exercise templates with names and descriptions.
        var exerciseTemplates = new List<(string Name, string Description)>
        {
            ("軽い散歩", "公園で朝または夜にリラックスして歩く。"),
            ("ジョギング", "心肺機能を向上させるために定期的に走る。"),
            ("自転車", "近所を自転車で走ったり、エアロバイクで運動する。"),
            ("水泳", "自由に泳いだり、ラウンドで全身を鍛える。")
        };

        for (int i = 0; i < totalExercises; i++)
        {
            var template = exerciseTemplates[random.Next(exerciseTemplates.Count)];

            exerciseList.Add(new Exercise
            {
                UserId = userId,
                ExerciseDate = GetRandomDate(random, dateTime),
                Name = template.Name,
                DurationMinutes = random.Next(20, 45),
                Calories = random.Next(50, 200),
                Description = template.Description,
                IsCompleted = random.Next(0, 2) == 1,
                CreatedUserId = userId,
            });
        }

        await _healtyDbContext.Exercise.AddRangeAsync(exerciseList);
        await _healtyDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// randomize date for exercise.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private DateTime GetRandomDate(Random random, DateTime date)
    {
        var isMorning = random.Next(2) == 0;

        int hour = isMorning ? random.Next(5, 8) : random.Next(17, 20);
        int minute = random.Next(0, 60);
        int second = random.Next(0, 60);

        return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
    }

    #endregion

    #region Dummy data Foods

    /// <summary>
    ///  automatically generate food list from food data source.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task SeedDataFoodList()
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

    #endregion

    #region Dummy data Recommend
    /// <summary>
    /// dummy image name
    /// </summary>
    private List<string> pathImage = new List<string>()
        {
            "2382431a-2149-4034-bc0a-7f14fea513f3.jpg",
            "29bf7322-9ab6-4b11-805f-4ffd2fea6830.jpg",
            "342a80e5-bc2b-48b4-b174-4c531dc17271.jpg",
            "2021df8f-60ee-42ae-9132-9fdcf7365266.jpg",
            "ff986521-183d-43bc-b207-1aa05114792d.jpg",
            "bcc41b91-1a92-457b-b07d-81fdc5f1cf10.jpg",
        };

    /// <summary>
    /// create 32 dummy data for recommend
    /// </summary>
    /// <returns></returns>
    private async Task SeedDataRecommend()
    {
        var tags = new List<string>()
            {
                "魚料理", "和食", "洋食", "中華料理", "イタリアン", "フレンチ", "スイーツ", "カフェ", "ベーカリー", "ラーメン", "DHA"
            };

        var recommends = new List<RecommendModel>();

        for (int i = 0; i < 32; i++)
        {
            recommends.Add(new RecommendModel
            {
                ImageUrl = pathImage[i % pathImage.Count],
                RecommendDate = DateTime.Now.AddDays(-i).ToString("yyyy.MM.dd HH:mm"),
                Content = $"おすすめの料理 {i + 1}",
                HashTag = tags.OrderBy(x => Guid.NewGuid()).Take(3).ToList()
            });
        }

        await _healtyDbContext.Recommends.AddRangeAsync(recommends.Select(r => new Recommend
        {
            ImageUrl = r.ImageUrl,
            Content = r.Content,
            HashTags = string.Join(",", r.HashTag),
            CreatedAt = DateTime.Now,
        }));

        await _healtyDbContext.SaveChangesAsync();
    }

    #endregion
}
