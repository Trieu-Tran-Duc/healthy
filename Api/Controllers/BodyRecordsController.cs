using ApplicationCore.Service;
using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/body_record")]
[ApiController]
public class BodyRecordsController : ControllerBase
{
    private readonly IBodyRecordsService _bodyRecordsService;

    public BodyRecordsController(IBodyRecordsService bodyRecordsService)
    {
        _bodyRecordsService = bodyRecordsService;
    }

    /// <summary>
    /// api insert diary for user.
    /// </summary>
    /// <param name="diaryRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("modify")]
    public async Task<IActionResult> InsertOrUpdateBodyRecordForUser([FromBody] InsertBodyRecordForUserDto requestRecord)
    {
        try
        {
            await _bodyRecordsService.InsertOrUpdateBodyRecordForUser(requestRecord);

            return StatusCode(201, $"Modify body record for {requestRecord.MailAddress} successfully.");
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
