using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IBodyRecordsService
{
    Task InsertOrUpdateBodyRecordForUser(InsertBodyRecordForUserDto requestRecord);
}

/// <summary>
/// body records service for managing body records.
/// </summary>
public class BodyRecordsService : IBodyRecordsService
{
    private readonly HealtyDbContext _dbContext;

    public BodyRecordsService(HealtyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// insert a body record for a user based on their email address.
    /// </summary>
    /// <param name="requestRecord"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task InsertOrUpdateBodyRecordForUser(InsertBodyRecordForUserDto requestRecord)
    {
        var userExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == requestRecord.MailAddress);

        if (userExists == null)
        {
            throw new ArgumentException($"User with mail address {requestRecord.MailAddress} does not exist.");
        }

        var existsBodyRecord = await _dbContext.BodyRecord
            .FirstOrDefaultAsync(br => br.UserId == userExists.Id && br.RecordDate.Date == requestRecord.RecordDate.Date);

        //update existing record if it exists
        if (existsBodyRecord != null)
        {
            existsBodyRecord.Weight = requestRecord.Weight;
            existsBodyRecord.UpdatedUserId = userExists.Id;

            _dbContext.BodyRecord.Update(existsBodyRecord);
            await _dbContext.SaveChangesAsync();
            return;
        }

        //insert new record if it does not exist
        var bodyRecord = new BodyRecords
        {
            UserId = userExists.Id,
            RecordDate = requestRecord.RecordDate,
            Weight = requestRecord.Weight,
            CreatedUserId = userExists.Id,
        };

        _dbContext.BodyRecord.Add(bodyRecord);
        await _dbContext.SaveChangesAsync();
        return;
    }
}
