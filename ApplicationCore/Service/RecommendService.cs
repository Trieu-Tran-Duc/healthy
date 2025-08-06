using Infrastructure.Data;
using Infrastructure.Models.Web;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Service
{
    public interface IRecommendService
    {
        Task<List<RecommendModel>> GetPaginatedRecommend(int pageIndex, int pageSize);
    }

    public class RecommendService : IRecommendService
    {
        private readonly HealtyDbContext _dbContext;

        public RecommendService(HealtyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// get paginated list of recommend
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<RecommendModel>> GetPaginatedRecommend(int pageIndex, int pageSize)
        {
            var recommends = await _dbContext.Recommends
                .Where(s => s.DeletedAt == null)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Content,
                    x.ImageUrl,
                    x.CreatedAt,
                    x.HashTags
                }).ToListAsync();

           return recommends
                .Select(x => new RecommendModel
                {
                    Content = x.Content,
                    ImageUrl = x.ImageUrl,
                    RecommendDate = x.CreatedAt.Value.ToString("yyyy.MM.dd HH:mm"),
                    HashTag = string.IsNullOrWhiteSpace(x.HashTags)
                                ? new List<string>()
                                : x.HashTags.Split(',')
                                            .Select(tag => tag.Trim())
                                            .Where(tag => !string.IsNullOrEmpty(tag))
                                            .ToList()
                })
                .OrderByDescending(x => x.RecommendDate)
                .ToList();
        }
    }
}
