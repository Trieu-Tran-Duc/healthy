using ApplicationCore.Service;
using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/diary")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly IDiaryService _diaryService;

        public DiaryController(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        /// <summary>
        /// api insert diary for user.
        /// </summary>
        /// <param name="diaryRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> InsertDiaryForUser([FromBody] InsertDiaryForUserDto diaryRequest)
        {
            try
            {
                await _diaryService.InsertDiaryForUser(diaryRequest);

                return StatusCode(201, $"Created diary for {diaryRequest.MailAddress} successfully.");
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
