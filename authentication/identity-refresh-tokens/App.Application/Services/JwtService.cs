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

    public ClaimsPrincipal ValidateToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["JwtSettings:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            var principal = handler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Invalid access token.", ex);
        }
    }

    public (string Sub, string Jti) ExtractSubAndJti(ClaimsPrincipal principal)
    {
        var sub = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(jti))
        {
            throw new SecurityTokenException("Access token missing required claims.");
        }
        return (sub, jti);
    }

    public (string Sub, string Jti) ValidateAndExtractClaims(string accessToken)
    {
        var principal = ValidateToken(accessToken);
        return ExtractSubAndJti(principal);
    }
}
