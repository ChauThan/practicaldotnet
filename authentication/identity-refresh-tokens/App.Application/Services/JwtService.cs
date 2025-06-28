using App.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Application.Services;

public class JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public (string AccessToken, DateTime Expiration, string JwtId) GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!)
        };

        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessTokenLifeTimeMinutes = Convert.ToDouble(_configuration["JwtSettings:AccessTokenLifeTimeMinutes"] ?? "1");
        var expiration = DateTime.UtcNow.AddMinutes(accessTokenLifeTimeMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var jwtId = token.Id;

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration, jwtId);
    }

    public RefreshToken GenerateRefreshToken(ApplicationUser user, string jwtId)
    {
        var refreshTokenLifeTimeDays = Convert.ToDouble(_configuration["JwtSettings:RefreshTokenLifeTimeDays"] ?? "3");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            JwtId = jwtId,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenLifeTimeDays),
            Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString()
        };
    }
}
