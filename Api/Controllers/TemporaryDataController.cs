using ApplicationCore.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/temporary-data")]
    [ApiController]
    public class TemporaryDataController : ControllerBase
    {
        private readonly ITemporaryDataService _temporaryDataService;

        public TemporaryDataController(ITemporaryDataService temporaryDataService)
        {
            _temporaryDataService = temporaryDataService;
        }

        /// <summary>
        /// seed temporary data for the application.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> SeedTemporaryData()
        {
            try
            {
                await _temporaryDataService.SeedDataAsync();

                return Ok(new { Message = "Temporary data seeded successfully.\\n You can log in using Email: test@gmail.com | Password: 123" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error.", Details = ex.Message });
            }
        }
    }
}
