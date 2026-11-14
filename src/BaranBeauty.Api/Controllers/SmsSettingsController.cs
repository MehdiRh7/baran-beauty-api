using BaranBeauty.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaranBeauty.Api.Controllers;

[ApiController,Authorize,Route("api/settings/sms")]
public sealed class SmsSettingsController(ISmsSettingsService service):ControllerBase
{
    [HttpGet] public Task<SmsSettingsDto> Get(CancellationToken ct)=>service.GetAsync(ct);
    [HttpPut] public async Task<ActionResult<SmsSettingsDto>> Save(SmsSettingsRequest request,CancellationToken ct)=>Ok(await service.SaveAsync(request,ct));
    [HttpPost("test")] public async Task<IActionResult> Test([FromBody] TestSmsRequest request,CancellationToken ct){await service.SendTestAsync(request.Receptor,ct);return NoContent();}
}
public sealed record TestSmsRequest(string Receptor);
