using ApplicationCore.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/food")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        /// <summary>
        /// generate a default food list for the application.
        /// </summary>
        /// <returns></returns>
        [HttpPost("seed")]
        public async Task<IActionResult> GenerateFoodList()
        {
            try
            {
                await _foodService.GenerateFoodList();

                return Ok(new { Message = $"Generate a default food list for the application successfully." });
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
