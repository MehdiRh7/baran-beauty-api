using BaranBeauty.Application;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace BaranBeauty.Api.Controllers;
[ApiController,Route("api/services")]public sealed class ServicesController(ISalonContentService content):ControllerBase
{
 [HttpGet]public Task<IReadOnlyList<ServiceDto>> Get(CancellationToken ct)=>content.GetServicesAsync(User.Identity?.IsAuthenticated==true,ct);
 [Authorize,HttpPost]public async Task<ActionResult<ServiceDto>> Create(ServiceRequest r,CancellationToken ct)=>Ok(await content.SaveServiceAsync(null,r,ct));
 [Authorize,HttpPut("{id:guid}")]public async Task<ActionResult<ServiceDto>> Update(Guid id,ServiceRequest r,CancellationToken ct)=>Ok(await content.SaveServiceAsync(id,r,ct));
 [Authorize,HttpDelete("{id:guid}")]public async Task<IActionResult> Delete(Guid id,CancellationToken ct){await content.DeleteServiceAsync(id,ct);return NoContent();}
}
