using ApplicationCore.Helper;
using ApplicationCore.Service;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly IAccessSessionHelper _accessSessionHelper;
        private readonly IExerciseService _exerciseService;
        private readonly IDiaryService _diaryService;
        private readonly ILogger<HomeController> _logger;

        private const int PAGE_SIZE = 8;

        public ChallengeController
        (
            IAccessSessionHelper accessSessionHelper, 
            IExerciseService exerciseService, 
            IDiaryService diaryService,
            ILogger<HomeController> logger
        )
        {
            _accessSessionHelper = accessSessionHelper;
            _exerciseService = exerciseService;
            _diaryService = diaryService;
            _logger = logger;
        }

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

        [HttpGet]
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
    }
}
