using ApplicationCore.Helper;
using ApplicationCore.Service;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("metrics")]
    public class MetricsController : Controller
    {
        private readonly IBodyMetricsService _bodyService;

        private readonly ILogger<HomeController> _logger;

        private readonly IAccessSessionHelper _accessSessionHelper;
        public MetricsController(IBodyMetricsService bodyService, ILogger<HomeController> logger, IAccessSessionHelper accessSessionHelper)
        {
            _bodyService = bodyService;
            _logger = logger;
            _accessSessionHelper = accessSessionHelper;
        }

        /// <summary>
        /// gernerate body metrics chart based on time metrics.
        /// </summary>
        /// <param name="timeMetrics"></param>
        /// <returns></returns>
        [HttpGet("render-body-chart")]
        public async Task<IActionResult> RenderBodyChart([FromQuery] TimeMetrics timeMetrics)
        {
            try
            {
                var sessionUser = await _accessSessionHelper.GetUserContextAsync();
                var bodyMetrics = await _bodyService.GetBodyMetricsAsync(sessionUser.UserId, timeMetrics);

                return Json(new { bodyMetrics.TimeLine, bodyMetrics.Weight, bodyMetrics.Body });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Load metrics error: ");
                throw;
            }
        }
    }
}
