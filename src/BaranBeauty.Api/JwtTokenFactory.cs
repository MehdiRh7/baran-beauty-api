using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BaranBeauty.Application;
using Microsoft.IdentityModel.Tokens;
namespace BaranBeauty.Api;
public interface IJwtTokenFactory { LoginResponse Create(AuthenticatedAdmin admin); }
public sealed class JwtTokenFactory(string key,string issuer,string audience,int minutes):IJwtTokenFactory
{
    public LoginResponse Create(AuthenticatedAdmin admin){var expires=DateTime.UtcNow.AddMinutes(minutes);var claims=new[]{new Claim(JwtRegisteredClaimNames.Sub,admin.Id.ToString()),new Claim(JwtRegisteredClaimNames.UniqueName,admin.Username),new Claim(ClaimTypes.Name,admin.DisplayName),new Claim(ClaimTypes.Role,"Admin"),new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())};var token=new JwtSecurityToken(issuer,audience,claims,expires:expires,signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token),expires,admin.DisplayName);}
}
