using ApplicationCore.Helper;
using ApplicationCore.Service;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Route("challenge")]
    public class ChallengeController : Controller
    {
        private readonly IAccessSessionHelper _accessSessionHelper;
        private readonly IExerciseService _exerciseService;
        private readonly IDiaryService _diaryService;
        private readonly IBodyMetricsService _bodyService;
        private readonly ILogger<HomeController> _logger;

        private const int PAGE_SIZE = 8;

        public ChallengeController
        (
            IAccessSessionHelper accessSessionHelper, 
            IExerciseService exerciseService, 
            IDiaryService diaryService,
            IBodyMetricsService bodyMetrics,
            ILogger<HomeController> logger
        )
        {
            _accessSessionHelper = accessSessionHelper;
            _exerciseService = exerciseService;
            _diaryService = diaryService;
            _bodyService = bodyMetrics;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentDate = DateTime.Now;
                var sessionUser = await _accessSessionHelper.GetUserContextAsync();
                var exercise = await _exerciseService.GetExercisesByUserId(sessionUser.UserId, currentDate);
                var diaries = await _diaryService.GetUserDiariesPaginated(sessionUser.UserId, PAGE_SIZE, 1);

                var viewModel = new ChallengeViewModel
                {
                    ExerciseDate = currentDate.ToString("yyyy.MM.dd"),
                    Exercises = exercise,
                    Diaries = diaries
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Challenge page error: ");
                throw;
            }
        }

        [HttpGet("load-more-diary")]
        public async Task<IActionResult> LoadMoreDiary(int pageIndex)
        {
            try
            {
                var sessionUser = await _accessSessionHelper.GetUserContextAsync();
                var diaries = await _diaryService.GetUserDiariesPaginated(sessionUser.UserId, PAGE_SIZE, pageIndex);

                if (diaries.Count == 0)
                {
                    return Content("");
                }

                return PartialView("_PartialDiary", diaries);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Load More Diary error: ");
                throw;
            }
        }

        [HttpGet("generation-chart")]
        public async Task<IActionResult> GenerationChart([FromQuery] TimeMetrics timeMetrics)
        {
            try
            {
                var sessionUser = await _accessSessionHelper.GetUserContextAsync();
                var bodyMetrics = await _bodyService.GetBodyMetricsAsync(sessionUser.UserId, timeMetrics);

                return Json(new { bodyMetrics.TimeLine, bodyMetrics.Weight, bodyMetrics.Body });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Load More Diary error: ");
                throw;
            }
        }
    }
}
