using ApplicationCore.Service;
using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/exercise")]
[ApiController]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// seed default exercises for a user based on their email address.
    /// </summary>
    /// <param name="requestxercises"></param>
    /// <returns></returns>
    [HttpPost("seed")]
    public async Task<IActionResult> InsertExercisesForUser([FromBody] InsertExercisesForUserDto requestxercises)
    {
        try
        {
            await _exerciseService.InsertExercisesForUser(requestxercises);

            return Ok(new { Message = $"Default exercises inserted for user has mail address {requestxercises.MailAddress} successfully." });
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

    /// <summary>
    /// get all exercises for a user by their user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetExercisesByUserId(int userId)
    {
        try
        {
            var exercises =  await _exerciseService.GetExercisesByUserId(userId);
           
            return Ok(new ApiResponse<List<GetExercisesByUserModel>>(
                  status: 200, 
                  message: "Get exercises successfully.", 
                  data: exercises)
            );
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

    /// <summary>
    /// update the status of exercises for a user by their user ID.
    /// </summary>
    /// <param name="requestxercises"></param>
    /// <returns></returns>
    [HttpPut("update_status")]
    public async Task<IActionResult> UpdateExercisesStatusUserById([FromBody] UpdateExercisesForUserDto requestxercises)
    {
        try
        {
            await _exerciseService.UpdateExercisesStatusUserById(requestxercises);

            return Ok(new { Message = $"Exercises updated for user has ID {requestxercises.UserId} successfully." });
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
