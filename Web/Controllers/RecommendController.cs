using ApplicationCore.Service;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("recommend")]
    public class RecommendController : Controller
    {
        private readonly ILogger<RecommendController> _logger;
        private readonly IRecommendService _recommendService;
        private const int PAGE_SIZE = 8;

        public RecommendController
        (
            ILogger<RecommendController> logger,
            IRecommendService recommendService
        )
        {
            _logger = logger;
            _recommendService = recommendService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var recommend = await _recommendService.GetPaginatedRecommend(1, PAGE_SIZE);    
                return View(recommend);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Recommend page error: ");
                throw;
            }
        }

        /// <summary>
        /// load more recommend for the recommend page.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("load-more-recommend")]
        public async Task<IActionResult> LoadMoreRecommend(int pageIndex)
        {
            try
            {
                var recommend = await _recommendService.GetPaginatedRecommend(pageIndex, PAGE_SIZE);

                if (recommend.Count == 0)
                {
                    return Content("");
                }

                return PartialView("_PartialRecommend", recommend);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Load More Recommend error: ");
                throw;
            }
        }
    }
}
