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

        public ChallengeController(IAccessSessionHelper accessSessionHelper, IExerciseService exerciseService)
        {
            _accessSessionHelper = accessSessionHelper;
            _exerciseService = exerciseService;
        }

        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            var sessionUser = await _accessSessionHelper.GetUserContextAsync();
            var exercise = await _exerciseService.GetExercisesByUserId(sessionUser.UserId, currentDate);

            var viewModel = new ChallengeViewModel
            {
                ExerciseDate = currentDate.ToString("yyyy.MM.dd"),
                Exercises = exercise
            };

            return View(viewModel);
        }
    }
}
