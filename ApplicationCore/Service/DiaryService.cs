using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IDiaryService
{
    Task InsertDiaryForUser(InsertDiaryForUserDto diaryRequest);
}

/// <summary>
/// directory service using for diary operations.
/// </summary>
public class DiaryService : IDiaryService
{
    private readonly HealtyDbContext _dbContext;

    public DiaryService(HealtyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Insert a diary entry for a user based on their email address.
    /// </summary>
    /// <param name="diaryRequest"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task InsertDiaryForUser(InsertDiaryForUserDto diaryRequest)
    {
        var userExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == diaryRequest.MailAddress);

        if (userExists == null)
        {
            throw new ArgumentException($"User with Email {diaryRequest.MailAddress} does not exist.");
        }

        _dbContext.Diaries.AddRange(new Diaries
        {
            UserId = userExists.Id,
            Title = diaryRequest.Title,
            Content = diaryRequest.Content,
        });

        await _dbContext.SaveChangesAsync();
    }
}
