using ApplicationCore.Service;
using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;

    private readonly IUserService _userService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController
    (
        ILogger<LoginController> logger, 
        IUserService userService, 
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> Login(string Email, string Password)
    {
        try
        {
            var userContext = await _userService.GetUserInfor(Email, Password);

            if (userContext == null)
            {
                return Json(new { success = false, message = "Invalid email or password." });
            }

            var context = _httpContextAccessor.HttpContext;
            context?.Session.Clear();
            await SetAsync(context.Session, nameof(GetUserModelDto), userContext);

            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = "Something error !!!!" });
        }

    }

    private static async Task SetAsync<T>(ISession session, string key, T value)
    {
        //https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/app-state?view=aspnetcore-2.2#session-state
        if (!session.IsAvailable)
            await session.LoadAsync();

        session.Set(key, JsonSerializer.SerializeToUtf8Bytes(value));
        await session.CommitAsync();
    }
}
