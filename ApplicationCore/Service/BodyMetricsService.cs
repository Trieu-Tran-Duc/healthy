using Infrastructure.Data;
using Infrastructure.Models.Web;
using Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service;

public interface IBodyMetricsService
{
    Task<BodyMetricsModel> GetBodyMetricsAsync(int userId, TimeMetrics timeMetrics);
}

/// <summary>
/// returns body metrics for a user.
/// </summary>
public class BodyMetricsService : IBodyMetricsService
{
    private readonly HealtyDbContext _dbContext;

    private readonly Dictionary<DayOfWeek, string> _japaneseDayNames = new Dictionary<DayOfWeek, string>
    {
        { DayOfWeek.Sunday,    "日曜日" },
        { DayOfWeek.Monday,    "月曜日" },
        { DayOfWeek.Tuesday,   "火曜日" },
        { DayOfWeek.Wednesday, "水曜日" },
        { DayOfWeek.Thursday,  "木曜日" },
        { DayOfWeek.Friday,    "金曜日" },
        { DayOfWeek.Saturday,  "土曜日" }
    };

    public BodyMetricsService(HealtyDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// gets body metrics for a user based on the specified time metrics.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="timeMetrics"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<BodyMetricsModel> GetBodyMetricsAsync(int userId, TimeMetrics timeMetrics)
    {
        switch (timeMetrics)
        {
            case TimeMetrics.Daily:
                return await GetMetricDaily(userId);
            case TimeMetrics.Week:
                return await GetMetricWeek(userId);
            case TimeMetrics.Month:
                return await GetMetricMonthly(userId);
            case TimeMetrics.Year:
                return await GetMetricYear(userId);
            default:
                throw new NotImplementedException($"Time metrics {timeMetrics} is not implemented.");
        }
    }

    /// <summary>
    /// get data chart weekly for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<BodyMetricsModel> GetMetricWeek(int userId)
    {
        var now = DateTime.Now;
        var startDate = now.AddDays(-7).Date;

        var weeklyWeights = await _dbContext.BodyRecord
            .Where(br => br.UserId == userId && br.DeletedAt == null && br.RecordDate >= startDate && br.RecordDate <= now)
            .AsNoTracking()
            .ToListAsync();

        var groupedWeights = weeklyWeights
            .GroupBy(br => br.RecordDate.DayOfWeek)
            .Select(g => new {
                Label = g.Key,
                Value = g.Average(x => x.Weight)
            })
            .OrderBy(x => (int)x.Label)
            .ToList();

        var weeklyBody = await _dbContext.Exercise
            .Where(br => br.UserId == userId
                && br.DeletedAt == null
                && br.IsCompleted
                && br.ExerciseDate >= startDate
                && br.ExerciseDate <= now)
            .AsNoTracking()
            .ToListAsync();

        var groupedBody = weeklyBody
            .GroupBy(br => br.ExerciseDate.DayOfWeek)
            .Select(g => new
            {
                Label = g.Key,
                Value = g.Average(x => x.Calories ?? 0)
            })
            .OrderBy(x => (int)x.Label)
            .ToList();

        var allDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>()
            .OrderBy(d => d == DayOfWeek.Sunday ? 7 : (int)d)
            .ToList();

        var metricWeightsDict = groupedWeights.ToDictionary(x => x.Label, x => x.Value);
        var bodyMetricsDict = groupedBody.ToDictionary(x => x.Label, x => x.Value);

        var orderedMetricWeights = allDays.Select(d => metricWeightsDict.GetValueOrDefault(d, 0)).ToList();
        var orderedBodyMetrics = allDays.Select(d => bodyMetricsDict.GetValueOrDefault(d, 0)).ToList();
        var timeline = allDays.Select(d => _japaneseDayNames[d]).ToList();

        return new BodyMetricsModel
        {
            Weight = orderedMetricWeights,
            Body = orderedBodyMetrics,
            TimeLine = timeline
        };
    }

    /// <summary>
    /// get data chart monthly for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<BodyMetricsModel> GetMetricMonthly(int userId)
    {
        var now = DateTime.Now;
        var startDate = now.AddMonths(-11).Date;

        var weightMetric = await _dbContext.BodyRecord
            .Where(br => br.UserId == userId && br.DeletedAt == null && br.RecordDate >= startDate)
            .AsNoTracking()
            .GroupBy(br => new { br.RecordDate.Year, br.RecordDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Value = g.Average(br => br.Weight) })
            .ToListAsync();

        var bodyMetric = await _dbContext.Exercise
            .Where(br => br.UserId == userId && br.IsCompleted && br.DeletedAt == null && br.ExerciseDate >= startDate)
            .AsNoTracking()
            .GroupBy(br => new { br.ExerciseDate.Year, br.ExerciseDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Calories = g.Average(br => br.Calories) ?? 0 })
            .ToListAsync();

        var last12Months = Enumerable.Range(0, 12)
            .Select(i => now.AddMonths(-11 + i))
            .Select(d => new
            {
                d.Year,
                d.Month,
                Label = $"{d.Month}月",
                Weight = weightMetric.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month)?.Value ?? 0,
                Calories = bodyMetric.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month)?.Calories ?? 0
            })
            .ToList();

        var timeLabels = last12Months.Select(x => x.Label).ToList();
        var weightData = last12Months.Select(x => x.Weight).ToList();
        var caloriesData = last12Months.Select(x => x.Calories).ToList();

        return new BodyMetricsModel
        {
            TimeLine = timeLabels,
            Weight = weightData,
            Body = caloriesData
        };
    }

    /// <summary>
    /// get data chart yearly for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<BodyMetricsModel> GetMetricYear(int userId)
    {
        var now = DateTime.Now;
        int currentYear = now.Year;
        int numberOfYears = 5;

        var targetYears = Enumerable.Range(currentYear - numberOfYears + 1, numberOfYears).ToList();

        var weightMetric = await _dbContext.BodyRecord
            .Where(br => br.UserId == userId && br.DeletedAt == null && targetYears.Contains(br.RecordDate.Year))
            .AsNoTracking()
            .GroupBy(br => br.RecordDate.Year)
            .Select(g => new { Year = g.Key, Value = g.Average(br => br.Weight) })
            .ToListAsync();

        var bodyMetric = await _dbContext.Exercise
            .Where(br => br.UserId == userId && br.DeletedAt == null && targetYears.Contains(br.ExerciseDate.Year))
            .AsNoTracking()
            .GroupBy(br => br.ExerciseDate.Year)
            .Select(g => new { Year = g.Key, Calories = g.Average(br => br.Calories) ?? 0 })
            .ToListAsync();
        
        var yearData = targetYears.OrderBy(y => y).Select(y => new
        {
            Label = $"{y}年",
            Weight = weightMetric.FirstOrDefault(w => w.Year == y)?.Value ?? 0,
            Calories = bodyMetric.FirstOrDefault(b => b.Year == y)?.Calories ?? 0
        }).ToList();

        return new BodyMetricsModel
        {
            TimeLine = yearData.Select(x => x.Label).ToList(),
            Weight = yearData.Select(x => x.Weight).ToList(),
            Body = yearData.Select(x => x.Calories).ToList()
        };
    }

    /// <summary>
    /// get daily body metrics for a user for the last 7 days.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<BodyMetricsModel> GetMetricDaily(int userId)
    {
        var now = DateTime.Now.Date;
        var startDate = now.AddDays(-6);

        var weightMetric = await _dbContext.BodyRecord
            .Where(br => br.UserId == userId && br.DeletedAt == null && br.RecordDate.Date >= startDate)
            .AsNoTracking()
            .GroupBy(br => br.RecordDate.Date)
            .Select(g => new { Date = g.Key, Weight = g.Average(br => br.Weight) })
            .ToListAsync();

        var bodyMetric = await _dbContext.Exercise
            .Where(br => br.UserId == userId && br.IsCompleted && br.DeletedAt == null && br.ExerciseDate.Date >= startDate)
            .AsNoTracking()
            .GroupBy(br => br.ExerciseDate.Date)
            .Select(g => new { Date = g.Key, Calories = g.Average(br => br.Calories) ?? 0 })
            .ToListAsync();

        var weightDict = weightMetric.ToDictionary(w => w.Date, w => w.Weight);
        var bodyDict = bodyMetric.ToDictionary(b => b.Date, b => b.Calories);

        var dailyData = Enumerable.Range(0, 7)
            .Select(i => startDate.AddDays(i))
            .Select(date => new
            {
                Label = date.ToString("yyyy/MM/dd"),
                Weight = weightDict.ContainsKey(date) ? weightDict[date] : 0,
                Calories = bodyDict.ContainsKey(date) ? bodyDict[date] : 0
            })
            .ToList();

        return new BodyMetricsModel
        {
            TimeLine = dailyData.Select(x => x.Label).ToList(),
            Weight = dailyData.Select(x => x.Weight).ToList(),
            Body = dailyData.Select(x => x.Calories).ToList()
        };
    }
}
