using ApplicationCore.Service;
using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// get all user information in the system.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllUserActive()
    {
        try
        {
            var user = await _userService.GetAllUserActive();

            return Ok(new ApiResponse<List<GetUserModelDto>>(
                   status: 200,
                   message: "list user active in the system.",
                   data: user)
             );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Internal server error.", Details = ex.Message });
        }
    }

    /// <summary>
    /// register a new user with email and password.
    /// </summary>
    /// <param name="userRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userRequest)
    {
        try
        {
            var newUser = await _userService.RegisterUser(userRequest);

            return StatusCode(201, $"User {newUser.FullName} with email {newUser.MailAddress} has been registered successfully.");
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
