using ApplicationCore.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/meals")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;
        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        /// <summary>
        /// automatically generate meals for a user based on their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMeals(string email)
        {
            try
            {
                await _mealService.GenerateMeals(email, DateTime.Now);
                return StatusCode(201, $"Meals for {email} on {DateTime.Now:yyyy-MM-dd} generated successfully.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error.", Details = ex.Message });
            }
        }
    }
}
