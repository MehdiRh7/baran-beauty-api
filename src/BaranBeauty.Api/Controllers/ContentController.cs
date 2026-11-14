using BaranBeauty.Application;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace BaranBeauty.Api.Controllers;
[ApiController,Route("api/content")]public sealed class ContentController(ISalonContentService content,IFileStorage files):ControllerBase
{
 [HttpGet("staff")]public Task<IReadOnlyList<StaffDto>> Staff(CancellationToken ct)=>content.GetStaffAsync(User.Identity?.IsAuthenticated==true,ct);
 [Authorize,HttpPost("staff")]public async Task<ActionResult<StaffDto>> AddStaff(StaffRequest r,CancellationToken ct)=>Ok(await content.SaveStaffAsync(null,r,ct));
 [Authorize,HttpPut("staff/{id:guid}")]public async Task<ActionResult<StaffDto>> UpdateStaff(Guid id,StaffRequest r,CancellationToken ct)=>Ok(await content.SaveStaffAsync(id,r,ct));
 [Authorize,HttpDelete("staff/{id:guid}")]public async Task<IActionResult> DeleteStaff(Guid id,CancellationToken ct){await content.DeleteStaffAsync(id,ct);return NoContent();}
 [HttpGet("certificates")]public Task<IReadOnlyList<CertificateDto>> Certificates(CancellationToken ct)=>content.GetCertificatesAsync(ct);
 [Authorize,HttpPost("certificates")]public async Task<ActionResult<CertificateDto>> AddCertificate(CertificateRequest r,CancellationToken ct)=>Ok(await content.SaveCertificateAsync(null,r,ct));
 [Authorize,HttpPut("certificates/{id:guid}")]public async Task<ActionResult<CertificateDto>> UpdateCertificate(Guid id,CertificateRequest r,CancellationToken ct)=>Ok(await content.SaveCertificateAsync(id,r,ct));
 [Authorize,HttpDelete("certificates/{id:guid}")]public async Task<IActionResult> DeleteCertificate(Guid id,CancellationToken ct){await content.DeleteCertificateAsync(id,ct);return NoContent();}
 [HttpGet("media")]public Task<IReadOnlyList<MediaDto>> Media(CancellationToken ct)=>content.GetMediaAsync(ct);
 [Authorize,RequestSizeLimit(100_000_000),HttpPost("media")]public async Task<ActionResult<MediaDto>> Upload(IFormFile file,[FromForm]string section,CancellationToken ct){await using var stream=file.OpenReadStream();var url=await files.SaveAsync(stream,file.FileName,file.ContentType,ct);var type=file.ContentType.StartsWith("video/")?"video":"image";return Ok(await content.AddMediaAsync(file.FileName,url,type,section,ct));}
 [Authorize,HttpDelete("media/{id:guid}")]public async Task<IActionResult> DeleteMedia(Guid id,CancellationToken ct){await content.DeleteMediaAsync(id,ct);return NoContent();}
 [HttpGet("salon")]public Task<SalonProfileDto?> Salon(CancellationToken ct)=>content.GetProfileAsync(ct);
 [Authorize,HttpPut("salon")]public async Task<ActionResult<SalonProfileDto>> SaveSalon(SalonProfileRequest r,CancellationToken ct)=>Ok(await content.SaveProfileAsync(r,ct));
}
