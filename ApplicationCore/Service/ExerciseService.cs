using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IExerciseService
{
    Task InsertExercisesForUser(InsertExercisesForUserDto requestxercises);
    Task<List<GetExercisesByUserModel>> GetExercisesByUserId(int userId, DateTime exerciseDate);
    Task UpdateExercisesStatusUserById(UpdateExercisesForUserDto requestxercises);
    Task<(double exerciseRate, string exerciseDate)> GetExerciseCompletionPercentageByUserId(int userId);
}

/// <summary>
/// service using for exercise operations.
/// </summary>
public class ExerciseService : IExerciseService
{
    private readonly HealtyDbContext _dbContext;

    public ExerciseService(HealtyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// get exercise completion percentage by user id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<(double exerciseRate,string exerciseDate)> GetExerciseCompletionPercentageByUserId(int userId)
    {
        var currentDate = DateTime.Now;

        var exerciesExists = await _dbContext.Exercise.AnyAsync(u => u.Id == userId);
        if( !exerciesExists)
        {
            return (0, currentDate.ToString("MM/dd"));
        }

        // Get all exercises for the user on the current date.
        var exercises = await _dbContext.Exercise
            .Where(e => e.UserId == userId && e.ExerciseDate.Date == currentDate.Date && e.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();

        var totalExercises = exercises.Count;
        var totalCompletedExercises = exercises.Count(e => e.IsCompleted);
        
        var exerciseRate = totalExercises == 0 ? 0 : (double)totalCompletedExercises / totalExercises * 100;
     
        return (exerciseRate, currentDate.ToString("MM/dd"));
    }

    /// <summary>
    /// get exercises by user id and exercise date.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="exerciseDate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<List<GetExercisesByUserModel>> GetExercisesByUserId(int userId, DateTime exerciseDate)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new ArgumentException($"User with Id {userId} does not exist.");
        }

        return await _dbContext.Exercise
            .Where(e => e.UserId == userId && e.DeletedAt == null && e.ExerciseDate.Date == exerciseDate.Date)
            .AsNoTracking()
            .Select(e => new GetExercisesByUserModel
            {
                Id = e.Id,
                Name = e.Name,
                DurationMinutes = e.DurationMinutes,
                Calories = e.Calories,
                Description = e.Description,
                ExerciseDate = e.ExerciseDate,
            })
            .OrderBy(e => e.Calories)
            .ToListAsync();
    }

    /// <summary>
    /// insert default exercises for user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task InsertExercisesForUser(InsertExercisesForUserDto requestxercises)
    {
        var userExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == requestxercises.MailAddress);

        if (userExists == null)
        {
            throw new ArgumentException($"User with mail address {requestxercises.MailAddress} does not exist.");
        }

        var exercises = GenerateDefaultExercises(userExists.Id, requestxercises.TotalExercises);

        _dbContext.Exercise.AddRange(exercises);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// update exercises status for user by id.
    /// </summary>
    /// <param name="requestxercises"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task UpdateExercisesStatusUserById(UpdateExercisesForUserDto requestxercises)
    {
        var currentExerciseUser = await _dbContext.Exercise
            .FirstOrDefaultAsync(u => u.UserId == requestxercises.UserId && u.Id == requestxercises.ExerciseId);

        if (currentExerciseUser == null)
        {
            throw new ArgumentException($"User with ID {requestxercises.UserId} does not exist.");
        }

        currentExerciseUser.IsCompleted = requestxercises.IsComplete;
        currentExerciseUser.UpdatedUserId = requestxercises.UserId;

        _dbContext.Exercise.Update(currentExerciseUser);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Automatically generated data exercise of user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private List<Exercise> GenerateDefaultExercises(int userId, int totalExercises)
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
                ExerciseDate = GetRandomDate(random, DateTime.Now),
                Name = template.Name,
                DurationMinutes = random.Next(20, 45),
                Calories = random.Next(50, 200),
                Description = template.Description,
                IsCompleted = false,
                CreatedUserId = userId,
            });
        }

        return exerciseList;
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
}
