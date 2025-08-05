using ApplicationCore.Helper;
using ApplicationCore.Service;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

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

    [HttpGet]
    public IActionResult GetChartData()
    {
        var labels = new[] { "6月", "7月", "8月", "9月", "10月", "11月", "12月", "1月", "2月", "3月", "4月", "5月" };
        var line1 = new[] { 90, 88, 75, 77, 65, 65, 55, 50, 47, 40, 38, 35 };
        var line2 = new[] { 91, 89, 70, 75, 72, 60, 68, 60, 55, 53, 52, 55 };

        return Json(new { labels, line1, line2 });
    }

    [HttpGet]
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
