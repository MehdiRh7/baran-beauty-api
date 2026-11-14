using BaranBeauty.Application;using BaranBeauty.Domain;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace BaranBeauty.Api.Controllers;
[ApiController,Route("api/appointments")]public sealed class AppointmentsController(IBookingService booking):ControllerBase
{
 [HttpPost]public async Task<ActionResult<AppointmentDto>> Create(AppointmentRequest r,CancellationToken ct)=>Ok(await booking.CreateAsync(r,ct));
 [Authorize,HttpGet]public Task<IReadOnlyList<AppointmentDto>> Get(CancellationToken ct)=>booking.ListAsync(ct);
 [Authorize,HttpPatch("{id:guid}/status")]public async Task<IActionResult> Status(Guid id,[FromBody]AppointmentStatus status,CancellationToken ct){await booking.ChangeStatusAsync(id,status,ct);return NoContent();}
 [HttpGet("blocked-dates")]public Task<IReadOnlyList<BlockedDate>> Blocked(CancellationToken ct)=>booking.GetBlockedDatesAsync(ct);
 [Authorize,HttpPost("blocked-dates")]public async Task<IActionResult> Block(BlockedDateRequest r,CancellationToken ct){await booking.SetBlockedDateAsync(r,ct);return NoContent();}
 [Authorize,HttpDelete("blocked-dates/{id:guid}")]public async Task<IActionResult> Unblock(Guid id,CancellationToken ct){await booking.RemoveBlockedDateAsync(id,ct);return NoContent();}
}
