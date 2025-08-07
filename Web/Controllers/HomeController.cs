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
    private readonly IBodyMetricsService _bodyMetrics;

    private const int PAGE_SIZE = 8;

    public HomeController
    (
        ILogger<HomeController> logger, 
        IAccessSessionHelper accessSessionHelper,
        IExerciseService exerciseService,
        IMealService mealService,
        IBodyMetricsService bodyMetrics
    )
    {
        _logger = logger;
        _accessSessionHelper = accessSessionHelper;
        _exerciseService = exerciseService;
        _mealService = mealService;
        _bodyMetrics = bodyMetrics;
    }

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

    [HttpGet("get-chart-data")]
    public async Task<IActionResult> GetChartData()
    {
        try
        {
            var sessionUser = await _accessSessionHelper.GetUserContextAsync();
            var bodyMetrics = await _bodyMetrics.GetBodyMetricsAsync(sessionUser.UserId, TimeMetrics.Month);

            return Json(new { bodyMetrics.TimeLine, bodyMetrics.Weight, bodyMetrics.Body });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get chart data failed");
            throw;
        }
    }

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
