using BaranBeauty.Application;using Microsoft.AspNetCore.Mvc;
namespace BaranBeauty.Api.Controllers;
[ApiController,Route("api/auth")]public sealed class AuthController(IAdminAuthentication auth,IJwtTokenFactory tokens):ControllerBase
{[HttpPost("login")]public async Task<ActionResult<LoginResponse>> Login(LoginRequest request,CancellationToken ct){var admin=await auth.AuthenticateAsync(request.Username,request.Password,ct);return admin is null?Unauthorized(new{message="نام کاربری یا رمز عبور اشتباه است."}):Ok(tokens.Create(admin));}}
