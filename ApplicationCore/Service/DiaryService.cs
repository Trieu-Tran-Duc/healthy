using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Infrastructure.Models.Web;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IDiaryService
{
    Task InsertDiaryForUser(InsertDiaryForUserDto diaryRequest);
    Task<List<DiariesModel>> GetUserDiariesPaginated(int userId, int pageSize, int pageIndex);
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
    /// get all diary entries for a user by their user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<DiariesModel>> GetUserDiariesPaginated(int userId, int pageSize, int pageIndex)
    {
        return await _dbContext.Diaries
            .Where(d => d.UserId == userId && d.DeletedAt == null)
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DiariesModel
            {
                Title = d.Title,
                Content = d.Content,
                Date = d.CreatedAt.Value.ToString("MM/dd/yyyy"),
                Time = d.CreatedAt.Value.ToString("HH:mm"),
            })
             .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Insert a diary entry for a user based on their email address.
    /// </summary>
    /// <param name="diaryRequest"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task InsertDiaryForUser(InsertDiaryForUserDto diaryRequest)
    {
        var userExists = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.MailAddress == diaryRequest.MailAddress);

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
