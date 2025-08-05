using ApplicationCore.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class NoticeController : Controller
    {
        private readonly IAccessSessionHelper _accessSessionHelper;
        private readonly ILogger<NoticeController> _logger;

        public NoticeController(IAccessSessionHelper accessSessionHelper, ILogger<NoticeController> logger)
        {
            _accessSessionHelper = accessSessionHelper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> LoadMoreNotice(int pageIndex)
        //{
        //    try
        //    {
        //        var sessionUser = await _accessSessionHelper.GetUserContextAsync();

        //        //if (meals.Count == 0)
        //        //{
        //        //    return Content("");
        //        //}

        //        return PartialView("_PartialFoodHistory", meals);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Load More Notice error: ");
        //        throw;
        //    }
        //}
    }
}
