using ApplicationCore.Helper;
using ApplicationCore.Service;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

[Route("home")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IAccessSessionHelper _accessSessionHelper;

    private readonly IExerciseService _exerciseService;

    private readonly IMealService _mealService;

    private const int PAGE_SIZE = 8;

    public HomeController
    (
        ILogger<HomeController> logger, 
        IAccessSessionHelper accessSessionHelper,
        IExerciseService exerciseService,
        IMealService mealService
    )
    {
        _logger = logger;
        _accessSessionHelper = accessSessionHelper;
        _exerciseService = exerciseService;
        _mealService = mealService;
    }

    /// <summary>
    /// index page for home, it will show the exercise completion percentage and meal history.
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var sessionUser = await _accessSessionHelper.GetUserContextAsync();

            var (exerciseRate, exerciseDate) = await _exerciseService.GetExerciseCompletionPercentageByUserId(sessionUser.UserId);
            var meals = await _mealService.GetMealsByUserId(sessionUser.UserId, PAGE_SIZE, 1, null);
            
            var homeViewModel = new HomePageViewModel
            {
                ExerciseRate = exerciseRate,
                HistoryMeals = meals,
                ExerciseDate = exerciseDate,
            };

            return View(homeViewModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Home page failed");
            throw;
        }
    }

    /// <summary>
    /// load more meal history for the user.
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="mealType"></param>
    /// <returns></returns>
    [HttpGet("load-more-meal-history")]
    public async Task<IActionResult> LoadMoreMealHistory(int pageIndex, MealType? mealType)
    {
        try
        {
            var sessionUser = await _accessSessionHelper.GetUserContextAsync();
            var meals = await _mealService.GetMealsByUserId(sessionUser.UserId, PAGE_SIZE, pageIndex, mealType);

            if (meals.Count == 0)
            {
                return Content("");
            }

            return PartialView("_PartialFoodHistory", meals);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LoadMoreMealHistory failed:");
            throw;
        }
    }
}
